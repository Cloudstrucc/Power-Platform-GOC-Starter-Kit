param ($username, $password, $commitmessage)
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass

# Configure Git
# git config --global user.email "youremail@example.com"
# git config --global user.name "Your Name"

# Set the repository URL
$repositoryUrl = "https://'$username':'$password'@dev.azure.com/cloudstrucc/Cloudstrucc-Project-Management/_git/GOC-THEME-V2"

# Clone the repository
git clone $repositoryUrl repo
Set-Location repo

# Make changes to your files here (e.g., modify or add files)
# Add and commit changes
git add .
git commit -m "$commitmessage"

# Push changes to the branch
git push origin main