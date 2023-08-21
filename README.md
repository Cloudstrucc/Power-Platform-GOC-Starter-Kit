# Government of Canada Power Platform StarterKit

[Link to technical documentation](https://csdocs.z9.web.core.windows.net/documentation/index.html)

## Introduction

This project hosts the work items, GIT repostories and release automation artefacts for the yourproject project. The project is comprised of the following three teams:

1. Team: members of this team can interact with the standard work items such as tasks and user stories. This includes functional analysts, project managers, and other "stakeholders".
2. Developers: comprised of users who will be developing releases for yourproject using the GIT repositories and pipelines. These folks will be able to leverage CI to move their work to a staging environment where it could be tested (this is also done automatically everyday at 3am)
3. Tech leads: comprised of lead developers (architects) who will approve pull-requests and full releases to downstream environments. Once developers are ready to release a work item(s) they will be response to issue a PR to the release branch which will trigger an email/notification to the yourproject tech leads to review and approve (or return with comments). Once approved, the work is deployed to the target environment.

## Process (development team)

You will be provided a branch to clone from the yourproject GIT repository (yourproject) locally using your editor of choice, however we recommend using Visual Studio Code with the Power Platform build tools installed (CLI)

1. The version you've cloned will be "linked" to your Dataverse development environment where you will be doing all your work in a silo
2. When you are ready to merge in your changes to the primary development environment, you simply issue a PR to the "Development" branch which will trigger a pipline that will import your patch and or portal code into the main development environment (unmanaged) where you can verify that your patch doesnt have any dependency issues or fails our conventions (or industry wide conventions).
3. Note that at 3am every evening, a pipeline executes that migrates your dedicated patch to the staging (release) environment. However you have the flexibility to trigger this manually by running the PowerPlatform-CI-Staging pipeline manually. This is important so that you can validate that your work is deployable in a managed environment in preperation for a full release (twice a sprint/sometimes more frequently)
4. When releases are issued to downstream environments such as QA, UAT, CUT, PREPROD and PROD, you will be asked to smoke test your work.

## Process (tech leads)

 You will be provided a branch to clone from the yourproject GIT repository (yourproject) locally using your editor of choice, however we recommend using Visual Studio Code with the Power Platform build tools installed (CLI)

1. The version you've cloned will be "linked" to your Dataverse development environment where you will be doing all your work in a silo
2. When you are ready to merge in your changes to the primary development environment, you simply issue a PR to the "Development" branch which will trigger a pipline that will import your patch and or portal code into the main development environment (unmanaged) where you can verify that your patch doesnt have any dependency issues or fails our conventions (or industry wide conventions).
3. Note that at 3am every evening, a pipeline executes that migrates your dedicated patch to the staging (release) environment. However you have the flexibility to trigger this manually by running the PowerPlatform-CI-Staging pipeline manually. This is important so that you can validate that your work is deployable in a managed environment in preperation for a full release (twice a sprint/sometimes more frequently)
4. Every (X) day during a sprint you will issue a PR to the QA branch which will trigger a pipeline that will clone the yourproject, EDSMP-Processes solutions in the main development environment and deploy to Staging (upgrade) and subsequently save the artefact for further deployments.
5. If 4 is successful, issue a PR to the UAT envrionment which will trigger a "release" pipeline that will deploy the artefacts generated in step 4 (this is done in the releases menu - and you can choose which release artefacts to deploy) to UAT and upgrade the version there as well (managed).
6. If 5 is successful, new patches will be provisioned in the main development environment and developers will need to execute step 1 (thus clone or pull the assigned branch with the newly created patch assigned to them)
7. If 4 is not successful, depending on who the owner of the patch / portal PR is, they will be respoonsible to resolve their deployment issue and either issue a manual PR or wait for the process to run again at 3am and step 5 is blocked (for that person's release artefacts only)
8. If 6 is not successful, depending on who the owner of the patch / portal PR is, they will be respoonsible to resolve their deployment issue and either issue a manual PR or wait for the process to run again at 3am and step 6 is blocked (for that person's release artefacts only)
9. Once UAT has been approved based on the release artefacts generated and deployed from the above steps, you will issue a PR to the PREPROD branch which will deploy all the artefacts that have been tested.
10. Once 9 is complete, smoke testing must occurr (automated and manual - *more info to come on test plans, OWASP, etc.*)
11. If step 10 is successful which means that the test suite has passed, issue a PR to the "Main" branch which will deploy the artefacts to production. Note that by this point, devs and analysts may have test many new features and deployed them to UAT, but that doesnt matter as you will be choosing the release artefacts that were in scope of the UAT cycle for this particular release thus the process of deployment is always continuous. The only time there is a sligh "freeze" is step 4 which are the official "release dates" to be defined during a sprint. Otherwise, releases are performed daily to staging and QA, and twice a sprint (or sometimes never, maybe once depending on the priorities) to UAT and even less frequently to PREPROD. PREPROD should be a mirror of production and the only time there is a significant delta between pre prod and prod is during an actual release (so step 9).

## Cloning the repository to your local terminal/git client/vs code

### Step 1: Set up a Personal Access Token (repeat this step if your PAT expires)

(If using Azure DevOps) To set up a personal access token (PAT) for cloning private repositories from Azure DevOps in your Windows PowerShell terminal/VS Code Terminal, follow these steps:

- Open your web browser and navigate to your Azure DevOps organization.
- Sign in to your account.
- Click on your profile picture in the top-right corner and select "Security".
- Under the "Personal access tokens" section, click on "New token".
- Fill in the required details:
  - Token name: Choose a descriptive name for your token.
  - Organization: Select the relevant organization.
  - Expiration: Choose an expiration date for the token.
  - Scopes: For cloning repositories, you'll need to select at least the "Code (Read)" scope.
- Click "Create" to generate the token.
**IMPORTANT: Copy and save the generated token somewhere secure, as you won't be able to see it again.**
- Clone Repository:
  - Open your Windows PowerShell terminal.
- Configure Git:
  - Run the following commands to configure Git to use your personal access token for authentication:

```console
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"
git config --global credential.helper manager-core
```

- Replace "Your Name" and "<your.email@example.com>" with your actual name and email.
- Clone Repository:
  - Navigate to the directory where you want to clone the private repository.
  - Run the following command to clone the repository using your PAT:
  
```console
git clone https://dev.azure.com/YourOrganization/YourProject/_git/YourRepo # the URL of your porject
```

- Replace "YourOrganization", "YourProject", and "YourRepo" with the actual names of your Azure DevOps organization, project, and repository.
- Authentication:
  - When prompted for authentication, paste the PAT you copied earlier.
- Complete Clone:
  - Once the authentication is successful, the cloning process should proceed, and you will have successfully cloned the private repository to your local machine.

**Remember to keep your personal access token secure and do not share it with anyone. If you suspect your token has been compromised, you can revoke it and generate a new one. Also, be aware that using PATs in this manner might not be the most secure option for all scenarios; consider other authentication methods if necessary.**

### Step 2: Re-authenticating your terminal/git client for subsequent Pulls and Clones when session expires and PAT token is configured (step 1)

1. At the root of the yourproject repo press the Clone button
2. In the left modal popup, press generate git credentials
3. In your VS Code, or Terminal (or which ever git client you are using), enter the following:

```console
git clone https://YOURUSERNAME:PASSWORDGENERATED@dev.azure.com/YourOrganization/YourProject/_git/YourRepo
```

## Release Pipeline Automation, CICD and QA

**Make sure you are comfortable with GIT, cloning the repository and issuig PR's to merge your local branch into the Main branch to update the repository before executing the pipeline**
In this diagram, the Developer provides the Dynamics 365 Solution Name, Data File, and Power Apps Portal to the DevOps Pipeline. The DevOps Pipeline stores the artifacts in the release for the Dev Environment, commits them to the GIT Repository, and stores the releasable artifacts in the Azure Artifact Storage.
The Release Manager issues the release to QA, UAT, PREPROD, and Production environments. The QA Team confirms the release for each environment. If the release is rejected, the QA Team notifies the Developer in the comment thread. The Developer then issues a new release which follows the same process as the initial release.


### Minor Pipeline Sequence (Patches)-Developers only, and they are accountable to validate their releases to staging

![Subject to minor updates](Documentation/images/SDD/ebd835c6e9ddb602a745874fdc45a1f5.png)

### Major Pipeline Sequence (Solution Clone - Full) - Tech Lead Accountability

![Subject to minor updates](Documentation/images/SDD/2f21a350a12d214885d9dea998486833.png)

## Environment map

*(EXAMPLE -> Typically you will want to show your environment map in your main project's readme.me file that is typically linked to the home page of your repository)*
This section provides a table of that describes each envrionment and each environment's depedent services like email integration, SharePoint integration, B2C (GCKey ingeration), pipelines (projects, repos and branches). As a developer you have acceess full sys admin access to development and staging is your testing ground to test your managed deployment using business personas. You need to ensuret that before we release yoor story or features to the extended team using the release pipeliines.

*Replace yourproject-{} and yourproject-{org}-{} with your own domains. Typically you will want to follow a convention such as this example.*

*Also this assumes that you have a pre production environment that will mimick production by being registered to the Canada.ca domain for production staging.*

| Environment Name | CRM URL | Portal URL | B2C App Registration ID | B2C Redirect URI | SharePoint Site URL | Synched Email for Outbound | Synched Email for Inbound | Type (Sandbox/Prod) | Environment Security Group | Release Pipeline SPN Name |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Dev | <https://yourproject-dev.crm3.dynamics.com/> | <https://yourproject-org-dev.powerappsportals.com/> | 9eba5fa7-89ea-4ecd-b5e9-4af3b241ddb5 | <https://yourproject-org-dev.powerappsportals.com/signin-openid_2> | <https://tenantsubdomain.sharepoint.com/sites/PowerPlatform-Development> | <Server>/ | <inbound@dev.contoso.com> | Dev | Dev Users | dev-sp |
| staging | <https://yourproject-staging.crm3.dynamics.com/> | <https://yourproject-org-staging.powerappsportals.com> | ffffffff-gggg-hhhh-iiii-jjjjjjjjjjjj | <https://yourproject-org-staging.powerappsportals.com/signin-oidc> | <https://tenantsubdomain.sharepoint.com/sites/test> | <test@tenantsubdomain.onmicrosoft.com> | <inbound@test.tenantsubdomain.onmicrosoft.com> | Staging | Staging Users | staging-sp |
| QA | <https://yourproject-qa.crm3.dynamics.com/> | <https://yourproject-org-qa.powerappsportals.com> | ffffffff-gggg-hhhh-iiii-jjjjjjjjjjjj | <https://yourproject-org-qa.powerappsportals.com/signin-oidc> | <https://tenantsubdomain.sharepoint.com/sites/test> | <qa@tenantsubdomain.onmicrosoft.com> | <inbound@qa.tenantsubdomain.onmicrosoft.com> | QA | QA Users | qa-sp |
| UAT | <https://yourproject-uat.crm3.dynamics.com/> | <https://yourproject-org-uat.powerappsportals.com/> | 11a3c66a-fa5d-47c8-9176-e4946f311611 | <https://yourproject-org-uat.powerappsportals.com/signin-aad-b2c_1> | <https://tenantsubdomain.sharepoint.com/sites/PowerPlatform-Development> | email-smtp.ca-central-1.amazonaws.com | <inbound@uat.tenantsubdomain.onmicrosoft.com> | UAT | UAT Users | uat-sp |
| Pre Prod | <https://yourproject-preprod.crm3.dynamics.com/> | <https://educanada-test.canada.ca/> | 11a3c66a-fa5d-47c8-9176-e4946f311611 | <https://yourproject-test.canada.ca/signin-aad-b2c_1> | <https://tenantsubdomain.sharepoint.com/sites/PowerPlatform-Development> | email-smtp.ca-central-1.amazonaws.com | <inbound@uat.tenantsubdomain.onmicrosoft.com> | Pre Prod | Pre Prod Users | preprod-sp |
| Prod | <https://yourproject.crm3.dynamics.com> | <https://yourproject.canada.ca/> | pppppppp-qqqq-rrrr-ssss-tttttttttttt | <https://yourproject.canada.ca/signin-oidc> | <https://tenantsubdomain.sharepoint.com/sites/prod> | <prod@tenantsubdomain.onmicrosoft.com> | <inbound@prod.tenantsubdomain.onmicrosoft.com> | Prod | Prod Users | prod-sp |

The App User leveraged to connect to each non production environment is the yourproject-System App User. The Application ID for this user is: 32423424234. To obtain the secret value to connect to the environment using the PAC CLI or XrmToolBox or even the web API if using a console app or other tool to for example test automation, you need to request this to the tech lead and obtain access to the yourproject KeyVault that stores the secret value. Note this is sensitive information and we rotate keys for non production environment every 24 months. Production keys are not available to anyone beyond the project owners and senior IM/IT leads. [The following link](https://yourkeyvaultsecreturl.com)) is the KeyVault resource that stores our secret for non prodution environment and is available to org users only and whoever requires to view this key needs to request access to this resource.

## Overall software/cloud architecture (example)

![Sotware Architecture-Draft - Subject to minor updates](Documentation/images/SDD/d2b7fb22cf85599b4a99ae89b6fe8ac6.png)

## Wizard Form Configuration architecture

![Subject to minor updates](Documentation/images/SDD/ec0585993640016494873593500476ec.png)

## Enterprise Grants & Contributions

*Details coming soon* - baseline data model solution (EGCS-DS) is available under the Solutions/Unmanged folder. The unpacked and packed (managed) versions coming with the documentation update in our next release. A second version of the customer-self-service portal for G&C will be published as well in our next release slated for August 30 - which includes G&C specific baseline functionality whereas the customer-self-service includes the functionality for any dataverse environment with the D365 Customer Service (basic and pro) licensing available.

## Dependencies

To ensure this theme is deployable in your environment you will need the following (minimum only, additonal dependencies required if you deploy DocFX, DevOps Pipelines, B2C and all other artifacts for the fulsome implementation of the entire architecture - described in the documentation)

1. Dynamics 365 Customer Service Licensing (basic, pro, or enterpise) - Minimum.
2. The unmanaged solutions contained in the Solutions/Unmanaged folder
3. The customer-self-service portal application installed in the target Dataverse environment