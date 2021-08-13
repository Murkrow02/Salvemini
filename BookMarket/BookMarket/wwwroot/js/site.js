//Form ajax submission
function postForm(form, success, redirect = "") {

    //Show loading if button is present
    try {
        $(".ld-ext-right")[0].classList.toggle('running');
        $('.ld-ext-right').prop('disabled', true);
    } catch{
        //Do nothing (popup request searchbook)
    }

    //Create form data
    var data = new FormData(form[0]);

    //Perform Ajax call to backend
    $.ajax({
        url: form.attr('action'),
        type: "POST",
        data: data,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.status === 'success') {
                //Success
                Swal.fire({
                    icon: 'success',
                    title: 'Successo',
                    text: success,
                }).then(result => {
                    //Redirect to dashboard
                    if (redirect !== "" && redirect !== null)
                    window.location.href = redirect;
                })
            } else if (response.status === 'unauthorized') {

                //UnAuthorized
                window.location.href = 'login';
            }
            else if (response.status === 'meetingcheck') {
                //Meeting check
                window.location.href = 'meetingcheck';
            }
            else {
                //Error, display error message
                Swal.fire({
                    icon: 'error',
                    title: 'Errore',
                    text: response.status,
                })
            }
            $(".ld-ext-right")[0].classList.toggle('running');
            $('.ld-ext-right').prop('disabled', false);

        },
        fail: function () {
            Swal.fire({
                icon: 'error',
                title: 'Errore',
                text: "Si è verificato un errore inaspettato, contattaci se il problema persiste",
            });
            $(".ld-ext-right")[0].classList.toggle('running');
            $('.ld-ext-right').prop('disabled', false);
        }
    })
}

//Switch beteen pages
//Get arrows div
var arrowsDiv = $(".list-arrows");
var arrowNext = $(".list-arrows .arrow-next");
var arrowPrev = $(".list-arrows .arrow-prev");
var pageText = $(".list-arrows span");
var body = document.getElementsByTagName("BODY")[0];

//Ajax download new results
function loadNextPage(nextPage) {
    $('#list').load(window.location.pathname + '?pag=' + nextPage, function (response, status, fromSearch = false) {

        //Error downloading more results
        if (status !== "success") {
            Swal.fire({
                icon: 'error',
                title: 'Errore',
                text: 'Non è stato possibile scaricare altri risultati',
            })
            body.classList.remove("disabledBtn");
            return;
        }

        //No more items
        if (response === null || response === "") {
            Swal.fire({
                icon: 'warning',
                title: 'Attenzione',
                text: 'Non ci sono più elementi da mostrare',
            })

            arrowNext.addClass("disabledBtn");
        } else {
            //Re enable right arrow if previously hidden
            arrowNext.removeClass("disabledBtn");
        }



        //Set new page value
        arrowsDiv.data("page", nextPage);
        for (var i = 0; i < pageText.length; i++) {
            pageText[i].textContent = "Pagina " + nextPage;
        }

        //Disable left arrow if first page
        if (nextPage === 1)
            arrowPrev.addClass("disabledBtn");
        else
            arrowPrev.removeClass("disabledBtn");

        //Empty searchbar
        var searchBar = $("list-search")[0];
        if (!fromSearch && searchBar !== null) searchBar.value = "";

        //Scroll top
        window.scrollTo(0, 0);

        //Check if disabling next arrow
        var itemsCount = $(".search-list-element").length;
        if (itemsCount < 100)
            arrowNext.addClass("disabledBtn");
        else
            arrowNext.removeClass("disabledBtn");

        //Re enable body
        body.classList.remove("disabledBtn");
    });
}


//Ajax search new results
var searching = false;
async function ajaxSearch(text, el) {
    //Get body
    var body = document.getElementsByTagName("BODY")[0];

    //Disable page while loading
    body.classList.add("disabledBtn");

    //Check if text changed in x time
    await sleep(500);
    if (text !== el.value)
        return;

    //Remove whitespaces
    var text_ = text.trim().split(" ").join("%20");

    ////Check if null
    //if (text_ === null || text_ === "") {
    //    //Fill with initial values
    //    loadNextPage(1);
    //    return;
    //}

    //Search
    searching = true;
    $('#list').load(window.location.pathname + '?handler=Search&text=' + text_ , async function (response, status) {

        //Error downloading more results
        if (status !== "success") {
            Swal.fire({
                icon: 'error',
                title: 'Errore',
                text: 'Non è stato possibile scaricare altri risultati, ricarica la pagina',
            })
            body.classList.remove("disabledBtn");
            await sleep(1000);
            searching = false;
            return;
        }

        //Disable arrows while searching
        arrowPrev.addClass("disabledBtn");
        arrowNext.addClass("disabledBtn");

        //Set new page value
        arrowsDiv.data("page", 1);
        for (var i = 0; i < pageText.length; i++) {
            pageText[i].textContent = "Pagina " + 1;
        }

        //Remove loading
        body.classList.remove("disabledBtn");

        //Used to prevent popup "No results" used when list loads empty
        await sleep(1000);
        searching = false;
    });
}

//Call ajax to load results
function changePage(direction) {
    //Disable page while loading
    body.classList.add("disabledBtn");

    //Get current page
    var currentPage = parseInt(arrowsDiv.data("page"));

    //Find next page
    var nextPage = 0;
    if (direction === "next") {
        nextPage = currentPage + 1;
    } else {
        nextPage = currentPage - 1;
    }

    //Load next page
    loadNextPage(nextPage);
}

//Function used to await delay in async function
function sleep(ms) {
    return new Promise((resolve) => setTimeout(resolve, ms));
}
