using System.IO;
using System;
using System.Text;
using System.Collections;

namespace ParkingUtilitaire
{
    public class Transaction
    {
        // Attributes membres.
        private int ip;                     // IP de l'émetteur.
        private int port;                   // Port de l'émetteur.
        private int numeroDeTransaction;    // Transaction du client.
        private DateTime heure;             // Heure de l'action ( Chiffre entre 0 et 24 ).
        private int placesLibres;           // Le nombre de places restantes.
        private int numeroTicket;           // Le dernier Numéro de ticket attribué.
        private Action action;
        private FileStream fs;
        private StreamReader lecteur;
        private StreamWriter ecrivain;
        private string chemin;
        private string cheminCopieParking;
        
        // Méthodes membres.
        // Ctor par défaut.
        public Transaction()
        {
            ip = 0;
            port = 0;
            numeroDeTransaction = 0;
            heure = DateTime.Now/*Convert.ToDateTime("00:00:00")*/;
            placesLibres = 99;
            numeroTicket = 0;
            action = Action.ENTETE;
            fs = null;
            lecteur = null;
            ecrivain = null;
            chemin = @"C:\Works\Parking\ParkingUtilitaire\Parking.txt";
            cheminCopieParking = @"C:\Works\Parking\ParkingUtilitaire\CopieParking.txt"; 
        }

        /// <summary>
        /// Méthode que créee le fichier de transactions.
        /// </summary>
        /// <returns>Entier avec la valeur du retour.</returns>
        public int CreerLeFichierTransactions()
        {
            int ret = 0;
            try
            {
                if (File.Exists(chemin) == true)
                {
                    // Ouverture du Fichier.   
                    fs = new FileStream(chemin, FileMode.Open, FileAccess.Read, FileShare.None);
                }

                if (fs != null)
                {
                    // Lire le fichier.
                    lecteur = new StreamReader(fs, ASCIIEncoding.Default);
                    string donnees = lecteur.ReadLine();
                    // Fermeture du fichier.
                    lecteur.Close();

                    string[] valeurs = donnees.Split(',');
                    this.ip = Int32.Parse(valeurs[0]);
                    this.port = Int32.Parse(valeurs[1]);
                    this.numeroDeTransaction = Int32.Parse(valeurs[2]);
                    this.heure = DateTime.Parse(valeurs[3]);
                    this.placesLibres = Int32.Parse(valeurs[4]);
                    this.numeroTicket = Int32.Parse(valeurs[5]);
                    // On teste seulement la présence d'ENTÊTE pour vérifier sa validité.
                    if (String.Compare(valeurs[6].Trim(), "ENTETE") != 0)
                    {
                        this.action = Action.ENTETE;
                        ret = 1;
                    }
                }
                else
                {
                    // Creation du fichier.
                    fs = new FileStream(chemin, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                    ecrivain = new StreamWriter(fs, ASCIIEncoding.Default);
                    // Ecrire dans le fichier.
                    ecrivain.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", this.ip, this.port, this.numeroDeTransaction, this.heure.ToShortTimeString(), this.placesLibres, this.numeroTicket,
                        action);
                    ecrivain.Flush();
                    ecrivain.Close();
                    ret = 1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur: {0}", e.Message);
            }
            finally
            {
                // Fermeture du Fichier.
                fs.Close();
            }            
            return ret;
        }

        /// <summary>
        /// Méthode que reserve un ticket en entrant une heure. 
        /// </summary>
        /// <returns>Entier avec la valeur du retour</returns>
        public int ReserverUnTicketEnEntrantUneHeure()
        {
            int ret = 0;
            try
            {
                if (File.Exists(chemin))
                {
                    // Ouverture du fichier en lecture.
                    fs = new FileStream(chemin, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    // Lire le nombre de places libres dans le premier record du fichier.
                    lecteur = new StreamReader(fs, ASCIIEncoding.Default);
                    
                    string donnees = lecteur.ReadLine();
                    string[] valeurs = donnees.Split(',');
                    this.heure = DateTime.Parse(valeurs[3]);
                    this.placesLibres = Int32.Parse(valeurs[4]);
                    this.numeroTicket = Int32.Parse(valeurs[5]);
                    if (String.Compare(valeurs[6].Trim(), "ENTETE") == 0)
                    {
                        if (File.Exists(cheminCopieParking))
                            File.Delete(cheminCopieParking);

                        this.placesLibres -= 1;
                        this.numeroTicket += 1;
                        this.action = Action.ENTETE;
                        
                        // Ouverture du fichier en ecriture et lecture.
                        FileStream fsCopieParking = new FileStream(cheminCopieParking, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
                        // Ecrire dans le fichier.
                        // Remplacer ce record par le suivant.
                        StreamWriter ecrivain = new StreamWriter(fsCopieParking, ASCIIEncoding.Default);
                        ecrivain.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", this.ip, this.port, this.numeroDeTransaction, this.heure, this.placesLibres, this.numeroTicket, this.action);
                        ecrivain.Flush();

                        while ((donnees = lecteur.ReadLine()) != null)      
                        {
                            Transaction transaction = new Transaction();
                            valeurs = null;
                            valeurs = donnees.Split(',');
                            transaction.heure = DateTime.Parse(valeurs[3]);
                            transaction.placesLibres = Int32.Parse(valeurs[4]);
                            transaction.numeroTicket = Int32.Parse(valeurs[5]);
                            if (String.Compare(valeurs[6].Trim(), "ENTETE") == 0)
                                transaction.action = Action.ENTETE;
                            else if (String.Compare(valeurs[6].Trim(), "RESERVATION") == 0)
                                transaction.action = Action.RESERVATION;
                            else if (String.Compare(valeurs[6].Trim(), "PAIEMENT") == 0)
                                transaction.action = Action.PAIEMENT;
                            else if (String.Compare(valeurs[6].Trim(), "SORTIE") == 0)
                                transaction.action = Action.SORTIE;
                            // Ecrire dans le CopieParking.
                            ecrivain.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", transaction.ip, transaction.port, transaction.numeroDeTransaction, transaction.heure, transaction.placesLibres,
                                transaction.numeroTicket, transaction.action);
                            ecrivain.Flush();
                        }

                        // Ecrire dans le fichier CopieParking.
                        ecrivain.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", this.ip, this.port, this.numeroDeTransaction, DateTime.Now, this.placesLibres, this.numeroTicket, Action.RESERVATION);
                        ecrivain.Flush();
                        this.CopierStream(fsCopieParking, fs);
                        ecrivain.Close();
                        fsCopieParking.Close();

                        // Elimine le fichier CopieParking.
                        File.Delete(cheminCopieParking);
                        ret = 1;
                    }

                    // Fermeture du fichier.
                    lecteur.Close();
                }
                else
                    Console.WriteLine("Fichier de transaction n'exist pas.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur: {0}", e.Message);
            }
            finally
            {
                // Fermeture du fichier.
                fs.Close();
            }
            return ret;
        }

        /// <summary>
        /// Lit les octets du flux actuel et les écrit dans le flux de destination.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        private void CopierStream(Stream source, Stream destination)
        {
            // Positionement des pointeurs au debut du stream.
            source.Seek(0, SeekOrigin.Begin);
            destination.Seek(0, SeekOrigin.Begin);

            /*
             * Pour utiliser la copie entre deux flux de données dans le framework .NET 3.5 et avant on utilise cette methode.
             * Reads the bytes from the current stream and writes them to the destination stream.
             */
            byte[] buffer = new byte[source.Length];
            int lecteur;
            while ((lecteur = source.Read(buffer, 0, buffer.Length)) > 0)
                destination.Write(buffer, 0, lecteur);
        }
        
        // Payer le ticket.
        public int PayerLeTicket(int numeroTicketAPayer, DateTime heureDePaiement)
        {
            int ret = 0; 
            if (File.Exists(chemin))
            {
                try
                {
                    // Ouverture du fichier en lecture.
                    fs = new FileStream(chemin, FileMode.Open, FileAccess.Read, FileShare.None);
                    int octetLus = 0;
                    ArrayList tableauOctet = new ArrayList();
                    string donnees = String.Empty;
                    string[] valeurs = null;
                    
                    // Parcourir le stream de la fin vers le debut.
                    for (long offset = 1; offset <= fs.Length; offset++)
                    {
                        // Positionement du pointeur en fin du fichier.
                        fs.Seek(-offset, SeekOrigin.End);
                        octetLus = fs.ReadByte();
                        // Tester s'il 'y a eu un NL line feed, new line '\n' code ASCII = 10 ou Carriage return '\r' code ASCII = 13.
                        if (octetLus != 10 && octetLus != 13)
                            tableauOctet.Add(Convert.ToChar(octetLus));
                        else
                        {
                            // Verifie s'il y a des élèments dans le tableau.
                            if (tableauOctet.Count > 0)
                            {
                                // Vider le string.
                                donnees = String.Empty;
                                valeurs = null;

                                // Inverser l'ordre dans l'ArrayList.
                                tableauOctet.Reverse();
                                // Copier l'array vers la string.
                                for (int i = 0; i < tableauOctet.Count; i++)
                                    donnees = donnees + tableauOctet[i].ToString();

                                valeurs = donnees.Split(',');
                                this.ip = Int32.Parse(valeurs[0].Trim());
                                this.port = Int32.Parse(valeurs[1].Trim());
                                this.numeroDeTransaction = Int32.Parse(valeurs[2].Trim());
                                this.heure = DateTime.Parse(valeurs[3].Trim());
                                this.placesLibres = Int32.Parse(valeurs[4].Trim());
                                this.numeroTicket = Int32.Parse(valeurs[5].Trim());
                                if (this.numeroTicket == numeroTicketAPayer)
                                    break;

                                // Nettoyer les élèments d'arraylist.
                                tableauOctet.Clear();
                            }
                        }
                    }

                    // Fermeture du fichier.
                    fs.Close();

                    // Verifie se le paiement n'etait déjà efectué.
                    if (String.Compare(valeurs[6].Trim(), "RESERVATION") == 0 && this.numeroTicket == numeroTicketAPayer)
                    {
                        // Ouverture du fichier en ecriture.
                        fs = new FileStream(chemin, FileMode.Append, FileAccess.Write, FileShare.None);
                        ecrivain = new StreamWriter(fs, ASCIIEncoding.Default);
                        // Ecrire dans le fichier.
                        ecrivain.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", this.ip, this.port, this.numeroDeTransaction, DateTime.Now, this.placesLibres, this.numeroTicket, Action.PAIEMENT);
                        ecrivain.Flush();
                        ecrivain.Close();

                        tableauOctet.Clear();
                        ret = 1;
                    }
                    else
                        Console.WriteLine("Numero de ticket a payer {0} invalide", numeroTicketAPayer);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erreur: {0}", e.Message);
                }
                finally
                {
                    // Fermeture du fichier.
                    //ecrivain.Close();
                    fs.Close();
                }
            }
            return ret;
        }

        public int SortirDuParking(int numeroTicketPaye, DateTime heureDeSortie)
        {
            /*
             * heureDeSortie para testar se ela depassou o tempo limite no parking
             */
            int ret = 0;
            if (File.Exists(chemin))
            {
                try
                {
                    if (String.Compare(this.ParcourirLeFichierDeLaFinVersLeDebut(numeroTicketPaye).Trim(), "PAIEMENT") == 0 && this.numeroTicket == numeroTicketPaye)
                    {
                        // Ouverture du fichier en lecture.
                        fs = new FileStream(chemin, FileMode.Open, FileAccess.Read, FileShare.None);
                        ecrivain = new StreamWriter(fs, ASCIIEncoding.Default);
                        // Ecrire dans le fichier.
                        ecrivain.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", this.ip, this.port, this.numeroDeTransaction, DateTime.Now, this.placesLibres, this.numeroTicket, Action.SORTIE);
                        ecrivain.Flush();
                        ecrivain.Close();

                        ret = 1;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erreur: {0}", e.Message);
                }
                finally
                {
                    // Fermeture du fichier.
                    fs.Close();
                }
            }
            return ret;
        }

        private /*void*/string ParcourirLeFichierDeLaFinVersLeDebut(int numeroTicketAPayer)
        {
            // ...
            string[] valeurs = null;

            if (File.Exists(chemin))
            {
                try
                {
                    // Ouverture du fichier en lecture.
                    fs = new FileStream(chemin, FileMode.Open, FileAccess.Read, FileShare.None);
                    int octetLus = 0;
                    ArrayList tableauOctet = new ArrayList();
                    string donnees = String.Empty;

                    // Parcourir le stream de la fin vers le debut.
                    for (long offset = 1; offset <= fs.Length; offset++)
                    {
                        // Positionement du pointeur en fin du fichier.
                        fs.Seek(-offset, SeekOrigin.End);
                        octetLus = fs.ReadByte();
                        // Tester s'il 'y a eu un NL line feed, new line '\n' code ASCII = 10 ou Carriage return '\r' code ASCII = 13.
                        if (octetLus != 10 && octetLus != 13)
                            tableauOctet.Add(Convert.ToChar(octetLus));
                        else
                        {
                            // Verifie s'il y a des élèments dans le tableau.
                            if (tableauOctet.Count > 0)
                            {
                                // Vider le string.
                                donnees = String.Empty;
                                valeurs = null;

                                // Inverser l'ordre dans l'ArrayList.
                                tableauOctet.Reverse();
                                // Copier l'array vers la string.
                                for (int i = 0; i < tableauOctet.Count; i++)
                                    donnees = donnees + tableauOctet[i].ToString();

                                valeurs = donnees.Split(',');
                                this.ip = Int32.Parse(valeurs[0].Trim());
                                this.port = Int32.Parse(valeurs[1].Trim());
                                this.numeroDeTransaction = Int32.Parse(valeurs[2].Trim());
                                this.heure = DateTime.Parse(valeurs[3].Trim());
                                this.placesLibres = Int32.Parse(valeurs[4].Trim());
                                this.numeroTicket = Int32.Parse(valeurs[5].Trim());
                                if (this.numeroTicket == numeroTicketAPayer)
                                    break;

                                // Nettoyer les élèments d'arraylist.
                                tableauOctet.Clear();
                            }
                        }
                    }

                    // Fermeture du fichier.
                    fs.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erreur: {0}", e.Message);
                }
                finally
                {
                    // Fermeture du fichier.
                    fs.Close();
                }
            }
            return valeurs.ToString();
            // ...
        }
        //string que o utilizador entra.
        //String.Compare valeurs[6].Trim, ! 0 .numeroTicket numeroTicketAPayer

        //String.Compare valeurs[6].Trim, 0 && 

        //-
        /*--*/
        /*ArrayList objet in tableauOctet*/

        //    -1, SeekOrigin.Current;

        //...
        //    fs new FileStream chemin, FileMode.Open, FileAccess.Read, FileShare.None;
        //    //new StreamReader fs, ASCIIEncoding.Default;

        //    buffer new byte fs.Length;
        //    ArrayList novaString new ArrayList;


        //    for posicao; offset > 0; offset--
        //        buffer offset 'n'
        //            // se igual sai do loop.

        //    //for i buffer.Length; i > 0; i--
        //...
    }

    // Enumeration.
    public enum Action
    {
        ENTETE = 1,
        RESERVATION = 2,
        PAIEMENT = 3,
        SORTIE = 4
    }
}
/*
using System;
using System.Collections;
using System.IO;
namespace SortedList
{
    class Program
    {
        static void Main(string[] args)
        {
            ArrayList stringList = new ArrayList();
            StreamReader stringReader = new StreamReader("c:\\OptionList.txt");
            string option = "";
            while ((option = stringReader.ReadLine()) != null)
            {
                stringList.Add(option);
            }
            stringReader.Close();
            stringReader.Dispose();
            stringList.TrimToSize();
            for (int i = stringList.Count - 1; i >= 0; --i)
            {
                Console.WriteLine(stringList[i]);
            }
            Console.ReadLine();
        }
    }
}
 */

//@CWorks\Parking\ParkingUtilitaire\Parking.doc
//@C:\Works\Parking\ParkingUtilitaire\Parking.doc
//@C:\Works\Parking\ParkingUtilitaire\Parking.doc

// '
// ' '
// ' '
/*
--------------------------------------------------------------------------
 auto-generated
     the code is regenerated.
 auto-generated
------------------------------------------------------------------------------

     <summary>
    A strongly-typed resource , lookg up localized s, etc.
     <summary>
    This class was auto-generated by the StronglyTypedResourceBuilder
    class via a tool like ResGen or Visual Studio.
    global System.CodeDom.Compiler.GeneratedCodeAttribute System.Resources.Tools.StronglyTypedResourceBuilder,
    global System.Diagnostics.DebuggerNonUserCodeAttribute
    global System.Runtime.CompilerServices.CompilerGeneratedAttribute
    internal class Resources

        static global System.Resources.ResourceManager resourceMan;

        static global::System.Globalization.CultureInfo resourceCulture;

        global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute, CA1811:AUncalledPrivateCode
        internal Resources

        <summary>
        Returns the cached ResourceManager instance used by class.
         <summary>
        global::System.ComponentModel.EditorBrowsableAttribute global::System.ComponentModel.EditorBrowsableState.Advanced
        internal static global::System.Resources.ResourceManager ResourceManager
            get
                    global::System.Resources.ResourceManager temp  global::System.Resources.ResourceManager, typeof .Assembly;

        <summary>
        Overrides the current threads CurrentUICulture property for all
           resource lookups strongly typed resource class.
         <summary>
        global::System.ComponentModel.EditorBrowsableAttribute global::System.ComponentModel.EditorBrowsableState.Advanced
        internal static global::System.Globalization.CultureInfo Culture
            get
            set
                resourceCulture value;
*/


/*
 * Anastasios Arvanitis
 Χρόνια Πολλά κορίτσι σου εύχομαι υγεία, ευτυχία και πολλές επιτυχίες... να σαι πάντα καλά émoticône smile
 */