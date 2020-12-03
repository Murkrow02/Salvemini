﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xabe.FFmpeg;

namespace SalveminiApi_core.Pages
{
    public class TalentModel : PageModel
    {
        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Cellular { get; set; }

        [BindProperty]
        public string Description { get; set; }

        [BindProperty]
        public IFormFile Video { get; set; }

        [BindProperty]
        public string IpAddress { get; set; }


        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;
        public TalentModel(Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            _env = env;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //Check values
            if(string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Cellular))
            {
                return new JsonResult(new { status = "Inserisci nome e numero di telefono" });
            }

            if(Description != null && Description.Length > 3000)
            {
                return new JsonResult(new { status = "Inserisci massimo 3000 caratteri per la tua presentazione" });
            }

            if (string.IsNullOrEmpty(IpAddress))
            {
                return new JsonResult(new { status = "Non è stato possibile identificare il dispositivo, contattaci se il problema persiste" });
            }

            if(Video == null || Video.Length < 1)
            {
                return new JsonResult(new { status = "Devi allegare un video per iscriverti " });
            }

#warning check video info

            var VideoFolder = _env.WebRootPath + $"/talent/{IpAddress}";
            var InfoPath = Path.Combine(VideoFolder, Name.Replace(" ", "_") + ".txt");
            var VideoPath = Path.Combine(VideoFolder, Video.FileName);

            //Check if folder exists, return error
            if (Directory.Exists(VideoFolder))
            {
                return new JsonResult(new { status = "Hai già caricato un video, per motivi di sicurezza accettiamo una sola candidatura da ogni dispositivo. Contattaci se ritieni si tratti di un errore" });
            }

            //Create directory
            Directory.CreateDirectory(VideoFolder);

            //Write text info
            System.IO.File.WriteAllText(InfoPath, $"Numero di telefono: {Cellular}\n\nPresentazione: \n{Description}");

            //Save video
            using (var fileStream = new FileStream(VideoPath, FileMode.Create))
            {
                await Video.CopyToAsync(fileStream);
            }



            //Check video too long
            var mediaInfo = await FFmpeg.GetMediaInfo(VideoPath);
            var videoDuration = mediaInfo.Duration;
            if (videoDuration > TimeSpan.FromSeconds(100))
                {
                    //Delete everything
                    Directory.Delete(VideoFolder, true);
                    return new JsonResult(new { status = "Il video selezionato supera il limite di 100 secondi, selezionane un altro" });
                }
           

           

            return new JsonResult(new { status = "success" });
        }

    }
}
