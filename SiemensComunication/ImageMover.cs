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
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            var imageFiles = Directory.GetFiles(sourceFolder, "*.jpg"); // Presupunem că imaginile sunt .jpg

            foreach (var file in imageFiles)
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(destinationFolder, fileName);
                File.Move(file, destFile);
                Console.WriteLine($"Moved {fileName} to {destinationFolder}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error moving images: {ex.Message}");
        }
    }
}
