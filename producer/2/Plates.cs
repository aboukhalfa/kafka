using System;

internal class Plates
{
    internal static string GetPlate()
    {
        var plate = string.Empty;
        var rnd = new Random();
        for(int i=0; i<6; i++)
        {
            var n = rnd.Next(36);
            char c;
            if (n > 9)
                c = (char)((int)'A'+n-10);
            else
                c = (char)((int)'0'+n);
            
            plate += c;
        }

        return plate;
    }
    
}