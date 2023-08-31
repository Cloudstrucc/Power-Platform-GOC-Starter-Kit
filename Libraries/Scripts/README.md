# Scripts Folder

Includes useful PowerShell & Bash scripts

## GenerateCRT.ps1

The purpose of this script is to generate a CRT file to send to ENTRUST Certificate Authority (CA) so that they issue a cert bundle to generate a TLS certificate(s) for Power Pages Sites, and/or Azure Front Door depending on your target architecture.

## GeneratePFX.ps1

This script will generate a PFX and output the path and strong password. For this to work, ensure that you've downloaded the cert bundle (zip file and extract) to a certs directory at the root of this repository. The folder structure s/b root/certs/Intermediate.crt, Root.crt, ServerCertificate.crt. The PFX file will also be stored in your certs folder if you run the script in the same folder that it resides in this repository. *IMPORTANT:**In the .gitignore, there is an entry to ignore the certs folder. Ensure that if you edit these scripts, not to store your PFX and bundle files in a public or even private repo. Also each time you run this script it will regenerate a new pfx with a new password which only outputs to your terminal and is meant to copy paste into a secure vault / system.*

## Delete.ps1

Helper script to quickly purge the PDF files at the root of each documentation sub section.
