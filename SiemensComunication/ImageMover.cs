using System;
using System.IO;

public class ImageMover
{
    private readonly string sourceFolder;
    private readonly string destinationFolder;

    public ImageMover(string sourceFolder, string destinationFolder)
    {
        this.sourceFolder = sourceFolder;
        this.destinationFolder = destinationFolder;
    }

    public void MoveImages()
    {
        try
        {
            var imageFiles = Directory.GetFiles(sourceFolder, "*.jpg"); // Presupunem că imaginile sunt .jpg

            if (imageFiles.Length == 0)
            {
                // Nu sunt fișiere de mutat, nu se creează niciun folder
                Console.WriteLine("No images to move.");
                return;
            }

            // Crearea unui folder temporar pe serverul destinație cu data și ora curentă
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var tempFolder = Path.Combine(destinationFolder, $"Temp_{timestamp}");

            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            foreach (var file in imageFiles)
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(tempFolder, fileName);
                File.Move(file, destFile);
                Console.WriteLine($"Moved {fileName} to {tempFolder}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error moving images: {ex.Message}");
        }
    }
}
