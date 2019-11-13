//using Microsoft.AspNet.SignalR;
//using Microsoft.AspNet.SignalR.Hubs;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace SalveminiApi.Hubs
//{
//    [HubName("FlappyHub")]
//    public class BroadcastHub : Hub
//    {
//        private readonly Broadcaster _broadcaster = Broadcaster.Instance;

//        public static List<FlappyGroup> StaticGroups = new List<FlappyGroup>();


//        public void CreateGroup(string code)
//        {
//            //Save group locally in the list
//            StaticGroups.Add(new FlappyGroup { Code = code, Utenti = new List<string> { Context.ConnectionId } });

//            //Create signalR group
//            Groups.Add(Context.ConnectionId, code);

//            //Notify success
//            Clients.Client(Context.ConnectionId).SendSuccess("Il gruppo è stato creato con successo"); return;
//        }

//        public void JoinGroup(string code, int idUtente)
//        {
//            try
//            {
//                //Check if user can join
//                var gruppo = StaticGroups.FirstOrDefault(x => x.Code == code);
//                if (gruppo == null) { Clients.Client(Context.ConnectionId).SendError("Il gruppo non è stato trovato"); return; }; //Group not found, notify the user
//                if (gruppo.Utenti == null || gruppo.Utenti.Count < 1) { StaticGroups.RemoveAll(x => x.Code == code); Clients.Client(Context.ConnectionId).SendError("Il gruppo è stato eliminato"); } //Group empty, notify the user and delete group
//                if (gruppo.Utenti.Count >= 2) { Clients.Client(Context.ConnectionId).SendError("Il gruppo è al completo"); return; }; //Group full, notify the user

//                //Add user to local list group
//                gruppo.Utenti.Add(Context.ConnectionId);

//                //Add user to SignalR group
//                Groups.Add(Context.ConnectionId, code);

//                //Report success to request user
//                Clients.Client(Context.ConnectionId).SendSuccess("Sei entrato nel gruppo"); return;
//            }
//            catch (Exception ex)
//            {
//                //Handle uncaught error
//                Clients.Client(Context.ConnectionId).SendError("Si è verificato un errore sconosciuto, riprova più tardi o contattaci se il problema persiste");
//                Helpers.Utility.saveCrash("Error joining FlappyGroup", ex.ToString());
//                return;
//            }

//        }

//        public void sendY(float y, string code)
//        {
//            ////Find group
//            //var gruppo = StaticGroups.FirstOrDefault(x => x.Code == code);

//            ////Find user to send position
//            //var user = gruppo.Utenti.Where(x => x != Context.ConnectionId).ToString();

//            ////Send new position
//            //Clients.Client(user).updateY(y);

//            Clients.All.updateY(y);
//        }


//        public override Task OnDisconnected(bool stopCalled)
//        {

//            return base.OnDisconnected(stopCalled);

//        }


//    }


//    public class FlappyGroup
//    {
//        public string Code { get; set; }
//        public List<string> Utenti { get; set; }
//    }


//    public class Broadcaster
//    {

//        private static readonly Lazy<Broadcaster> _instance =
//            new Lazy<Broadcaster>(() => new Broadcaster(GlobalHost.ConnectionManager.GetHubContext<BroadcastHub>().Clients));

//        public static Broadcaster Instance => _instance.Value;

//        public IHubConnectionContext<dynamic> Clients { get; set; }

//        public Broadcaster(IHubConnectionContext<dynamic> clients)
//        {
//            Clients = clients;
//        }
//    }
//}