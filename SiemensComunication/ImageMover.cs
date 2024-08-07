public class ImageMover
{
    private readonly string _sourceFolder;
    private readonly string _destinationFolder;

    public ImageMover(string sourceFolder, string destinationFolder)
    {
        _sourceFolder = sourceFolder;
        _destinationFolder = destinationFolder;
    }

    public async Task MoveImagesAsync()
    {
        try
        {
            var imageFiles = Directory.GetFiles(_sourceFolder, "*.jpg"); // Presupunem că imaginile sunt .jpg

            if (imageFiles.Length == 0)
            {
                // Nu sunt fișiere de mutat, nu se creează niciun folder
                GlobalLogger.Logger.Information("No images to move.");
                return;
            }

            // Crearea unui folder temporar pe serverul destinație cu data și ora curentă
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var tempFolder = Path.Combine(_destinationFolder, $"Temp_{timestamp}");

            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            foreach (var file in imageFiles)
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(tempFolder, fileName);
                await Task.Run(() => File.Move(file, destFile));
                GlobalLogger.Logger.Information($"Moved {fileName} to {tempFolder}");
            }
        }
        catch (Exception ex)
        {
            GlobalLogger.Logger.Error($"Error moving images: {ex.Message}");
        }
    }
}
