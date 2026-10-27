using System;
using System.IO;

namespace ExceptionsDemo;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Start av programmet ===");

        // Skapar en mapp för testfilerna
        string mapp = Path.Combine(AppContext.BaseDirectory, "TestFiles");
        Directory.CreateDirectory(mapp);

        // Skapar filer med olika innehåll
        File.WriteAllText(Path.Combine(mapp, "ok.txt"), "4");
        File.WriteAllText(Path.Combine(mapp, "text.txt"), "abc");
        File.WriteAllText(Path.Combine(mapp, "noll.txt"), "0");
        File.WriteAllText(Path.Combine(mapp, "tom.txt"), "");

        // Testar olika situationer
        TestaFil("Filen fungerar", Path.Combine(mapp, "ok.txt"));
        TestaFil("Filen saknas", Path.Combine(mapp, "saknas.txt"));
        TestaFil("Fel text i filen", Path.Combine(mapp, "text.txt"));
        TestaFil("Talet är noll", Path.Combine(mapp, "noll.txt"));
        TestaFil("Filen är tom", Path.Combine(mapp, "tom.txt"));
        TestaFil("Tomt filnamn", "");

        Console.WriteLine("\nProgrammet är klart.");
    }

    static void TestaFil(string rubrik, string filnamn)
    {
        Console.WriteLine("\n--- " + rubrik + " ---");

        try
        {
            double resultat = ProcessFile(filnamn);
            Console.WriteLine("Resultat: " + resultat);
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine("Filen hittades inte: " + ex.Message);
        }
        catch (IOException ex)
        {
            Console.WriteLine("Problem med filen: " + ex.Message);
        }
        catch (FormatException ex)
        {
            Console.WriteLine("Texten är inte ett tal: " + ex.Message);
        }
        catch (DivideByZeroException ex)
        {
            Console.WriteLine("Kan inte dela med noll: " + ex.Message);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("Felaktigt filnamn: " + ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine("Fel vid läsning av filen: " + ex.Message);

            if (ex.InnerException != null)
            {
                Console.WriteLine("Ursprungligt fel: "
                    + ex.InnerException.GetType().Name);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ett oväntat fel inträffade: "
                + ex.Message);
        }
        finally
        {
            Console.WriteLine("Testet är avslutat.");
        }
    }

    static double ProcessFile(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("Filnamnet får inte vara tomt.");
        }

        StreamReader? reader = null;

        try
        {
            reader = new StreamReader(fileName);

            string? rad = reader.ReadLine();

            if (rad == null)
            {
                throw new InvalidOperationException("Filen är tom.");
            }

            int tal = int.Parse(rad);

            if (tal == 0)
            {
                throw new DivideByZeroException("Talet är noll.");
            }

            return 100.0 / tal;
        }
        catch (IOException)
        {
            // Skickar vidare felet till TestaFil
            throw;
        }
        catch (FormatException)
        {
            // Texten gick inte att göra om till ett tal
            throw;
        }
        catch (DivideByZeroException)
        {
            throw;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            // Sparar det ursprungliga felet
            throw new InvalidOperationException(
                "Det gick inte att läsa och räkna med filen.", ex);
        }
        finally
        {
            // Stänger filen även om något går fel
            if (reader != null)
            {
                reader.Close();
            }
        }
    }
}