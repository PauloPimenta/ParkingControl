using System;

namespace ParkingUtilitaire
{
    public class CreaTransacServeur
    {
        // Attributes membres.
        private int option;
        private Transaction transaction;

        // Methodes membres.
        // Ctor par défaut.
        public CreaTransacServeur() 
        {
            option = 0;
            transaction = new Transaction();
        }

        public void Menu()
        {
            do
            {
                do
                {
                    Console.Clear();
                    Console.WriteLine("1. Créer le fichier transactions");
                    Console.WriteLine("2. Réserver un ticket");
                    Console.WriteLine("3. Payer le ticket");
                    Console.WriteLine("4. Sortir du Parking");
                    Console.WriteLine("5. Lister le contenu du fichier");
                    Console.WriteLine("6. Quitter");
                    Console.Write("Entrez la option: ");
                    option = Convert.ToInt16(Console.ReadLine());
                }
                while (option < 1 || option > 6);

                int numeroTicket;
                string heure = String.Empty;
                switch (option)
                {
                    case 1:
                        int creerFichier = transaction.CreerLeFichierTransactions();
                        if (creerFichier == 1)
                            Console.WriteLine("Fichier de transition créee avec succes. ");
                        else
                            Console.WriteLine("Fichier de transition exist déjà. ");

                        Console.ReadLine();
                        break;
                    case 2:
                        int reserverTicket = transaction.ReserverUnTicketEnEntrantUneHeure();
                        if (reserverTicket == 1)
                            Console.WriteLine("Ticket reservé. ");
                        else
                            Console.WriteLine("Erreur dans la reservation du ticket. ");

                        Console.ReadLine();
                        break;
                    case 3:
                        Console.Write("Entrez le numéro de ticket: ");
                        numeroTicket = int.Parse(Console.ReadLine());
                        Console.Write("Entrez l'heure de paiement: ");
                        heure = Console.ReadLine();
                        DateTime heurePaiement = Convert.ToDateTime(/*heure*/"2016-03-24 3:09:26 PM");
                        int payerTicketPaiement = transaction.PayerLeTicket(numeroTicket, heurePaiement);
                        if (payerTicketPaiement == 1)
                            Console.WriteLine("Ticket payé. ");
                        else
                            Console.WriteLine("Erreur dans le paiement du ticket. ");
                            
                        Console.ReadLine();
                        break;
                    case 4:
                        Console.Write("Entrez le numéro de ticket: ");
                        numeroTicket = int.Parse(Console.ReadLine());
                        Console.Write("Entrez l'heure de sortie: ");
                        DateTime heureSortie = Convert.ToDateTime("2016-04-13 1:55:24 AM");
                        int payerTicketSortie = transaction.SortirDuParking(numeroTicket, heureSortie);
                        if (payerTicketSortie == 1)
                            Console.WriteLine("Vous pouvez sortir du parking. ");
                        else
                            Console.WriteLine("Vous avez depasser le temps limite pour sortir du parking. ");

                        Console.ReadLine();
                        break;
                    case 5:
                        break;
                }
            }
            while (option != 6);
        }

        public static void Main()
        {
            CreaTransacServeur cts = new CreaTransacServeur();
            // Affichage du menu.
            cts.Menu();
        }
        //string 
        //valeurs donnees.Split ';

        //2016-03-24 30929 PM;

        //new DateTime/*2000, 1, 1, heure, 0, 0*/;

        //public Demander le numéro de ticket a payer & L’heure de payement
    }
}

//-

/*to COM components.  If you need to access a type in assembly from 

      General Information about an assembly is controlled through the following 

/*using System.Windows;

Setting ComVisible to false makes the types in assembly not visible 
 COM, set the ComVisible attribute to on that type.
assembly: ComVisible false

In order to begin building localizable applications, set 
UICulture CultureYouAreCodingWith<UICulture> in your .csproj file
inside a <PropertyGroup>.  For example, you are using US english
in your source files, set the <UICulture> to en US.  Then uncomment
the NeutralResourceLanguage attribute below.  Update the en-US in
the line below to match the UICulture setting in the project file.

//assembly: NeutralResourcesLanguage en-US, UltimateResourceFallbackLocation.Satellite


assembly: ThemeInfo    ResourceDictionaryLocation.None, //where theme specic resource dictionaries are located
   // used a resource is not found in the page, 
   // or application resource dictionaries
   ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
   //used a resource is not found in the page, 

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specy all the values or you can default the Build and Revision Numbers 
// by using the * as shown below:
// assembly: AssemblyVersion 1.0.*
assembly: AssemblyVersion 1.0.0.0
assembly: AssemblyFileVersion 1.0.0.0
*/