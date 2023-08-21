# Set the path to the root folder (e.g., "documentation")
$rootFolderPath = "Documentation/documentation"

# Get a list of all PDF files in subfolders
$pdfFiles = Get-ChildItem -Path $rootFolderPath -Recurse -Include "*.pdf"

# Loop through each PDF file and delete it
foreach ($pdfFile in $pdfFiles) {
    $pdfFilePath = $pdfFile.FullName

    # Uncomment the next line to print the file path before deleting
    # Write-Host "Deleting: $pdfFilePath"
    
    Remove-Item -Path $pdfFilePath -Force
}

Write-Host "Deleted all PDF files."
