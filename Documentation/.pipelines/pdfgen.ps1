# Set the path to your documentation folder
$documentationPath = "..\documentation"
$documentationPathSite = "..\"

# Get a list of all .md files in subfolders
$markdownFiles = Get-ChildItem -Path $documentationPath -Recurse -Include "*.md"
$markdownFilesSite = Get-ChildItem -Path $documentationPath -Recurse -Include "*.md"

# Set the path to the Pandoc executable (change as needed)
$pandocPath = "pandoc"
$pdflatexPath = "C:\Users\Fred\AppData\Local\Programs\MiKTeX\miktex\bin\x64\pdflatex.exe"
# Loop through each .md file and generate PDF for md files
foreach ($file in $markdownFiles) {
    $inputFilePath = $file.FullName
    $outputFilePath = [System.IO.Path]::ChangeExtension($inputFilePath, "pdf")

    # Construct the Pandoc command
    $pandocCommand = "$pandocPath `"$inputFilePath`" -o `"$outputFilePath`" --pdf-engine=`"$pdflatexPath`""

    # Execute the Pandoc command
    Invoke-Expression $pandocCommand

    Write-Host "Generated PDF: $outputFilePath"
}
# Loop through each .md file and generate PDF for _site
foreach ($file in $markdownFilesSite) {
    $inputFilePath = $file.FullName
    $outputFilePath = Join-Path -Path $documentationPathSite -ChildPath "_site\documentation\$($file.Directory.Name)\$($file.BaseName).pdf"

    # Construct the Pandoc command
    $pandocCommand = "$pandocPath `"$inputFilePath`" -o `"$outputFilePath`" --pdf-engine=`"$pdflatexPath`""

    # Execute the Pandoc command
    Invoke-Expression $pandocCommand

    Write-Host "Generated PDF: $outputFilePath"
}


