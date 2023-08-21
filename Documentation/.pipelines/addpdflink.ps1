# Specify the root directory where your Markdown files are located
$rootDirectory = "..\documentation"

# Get a list of all Markdown files
$markdownFiles = Get-ChildItem -Path $rootDirectory -Filter "*.md" -Recurse

foreach ($file in $markdownFiles) {
    # Read the content of the Markdown file
    $content = Get-Content -Path $file.FullName

    # Find the first heading in the Markdown file
    $heading = $content | Select-String -Pattern "^#\s+(.*)" | ForEach-Object { $_.Matches.Groups[1].Value }

    if ($heading -ne $null) {
        # Generate the link to the PDF file
        $pdfLink = "[Download PDF](./$($file.BaseName).pdf)"

        powershell

        if ($content -match "^\[Download PDF\]\(.*\.pdf\)") {
            # Update the href value of the existing PDF link and ensure a blank line before and after
            $newContent = $content -replace "(^\[Download PDF\]\()(.*\.pdf)(\))", "`$1./$($file.BaseName).pdf`$3"
        
            # Remove any existing blank lines before the link
            $newContent = $newContent -replace "(?<!`r`n)^\[Download PDF\]\(.*\.pdf\)`r`n", "$1"
        
            # Remove any existing blank lines after the link
            $newContent = $newContent -replace "`r`n^\[Download PDF\]\(.*\.pdf\)(?!`r`n)", "$1"
        
            # Ensure only one blank line before and after the link
            $newContent = $newContent -replace "(`r`n){3,}", "`r`n`r`n"
            $newContent = $newContent -replace "(`r`n){2}(^\[Download PDF\]\(.*\.pdf\))", "`r`n$2"
            $newContent = $newContent -replace "(^\[Download PDF\]\(.*\.pdf\))(`r`n){2}", "$1`r`n"
             # Add a blank line after the last statement in the if block
 
        }
          
         else {
            # Add the PDF link below the first heading with a single blank line before and after
            $newContent = $content -replace "^#\s+$heading", "# $heading`r`n`$pdfLink`r`n"
        }

        # Write the updated content back to the file
        $newContent | Set-Content -Path $file.FullName
    }
}
