# Government of Canada Power Platform Starter Kit

[Link to quick setup for WET Canada.ca in a Power Pages Site](https://github.com/Cloudstrucc/Public)

[Link to technical documentation](https://csdocs.z9.web.core.windows.net/documentation/index.html)

## Introduction

The Government of Canada's integration of PBMM Azure and M365 for Government reflects a progressive shift towards modernization. By embracing SaaS and PaaS models over the traditional IaaS approach, the government attains streamlined operations and resource optimization. This transition empowers the government to prioritize innovation and citizen services, while abstracting infrastructure complexities through PaaS for faster development and scalability. This strategic approach also underlines the Government's role in enhancing efficiency, cost-effectiveness, and responsiveness, thus establishing and enabling more advanced and service-centric applications for both internal users, and external citizens and organizations leveraging its services in today's digital landscape.

The incorporation of the Power Platform within this context also amplifies the advantages of the SaaS approach, bolstering the Government of Canada's digital transformation. With a SaaS model, the Power Platform offers a predictable cost structure, eliminating the uncertainties associated with traditional infrastructure management whose pricing structure is often based on volume and other sometimes difficult to predict metrics. The kit also includes prebuilt CRM applications which also expedites the implementation of citizen-centric services, internal processing of the data/business processes, and enhancing engagement and satisfaction from users. Additionally, the inclusion of Power Pages Website empowers Government Departments and Agencies to swiftly establish a compelling online presence without intricate development efforts. The website has also been tested against WCAG accessibility tools and has undergone several accessiblity scans from various organizations. This amalgamation of the Power Platform's capabilities not only aligns seamlessly with the government's modernization goals but also augments its ability to swiftly adapt to evolving needs while efficiently utilizing resources.

## Starter kit

The kit offers a versatile template for PBMM-driven Power Platform projects. It caters to Power Pages websites, fortified with OIDC Single Sign On integration, including GCCF consolidator, Sign in Canada, standalone Azure AD, or B2C. It also ships with a CRM solution that enhances the platform features such as (but not exclusive to) governing web publishing for verticals like Grants, Contributions, Permits, and Regulatory/Compliance forms. The kit introduces an advanced form wizard, addressing multi-step form limitations, and its WCAG 2.x-compliant theme ensures accessibility (integrated with the WET Canada.ca framework). The documentation site can serve as a template for technical documentation and security protocols. The subsequent section outlines a template for repository overviews, embracing modern ALM practices. Noting this inaugural open-source release, expect minor and major updates, distinct stable/preview builds, and contributions welcomed from developers. Services with SLA support expedite stack integration, fostering collaboration for technological excellence. Accompanying CRM solutions and various optional components enhance this core structure. Noteworthy is the automated pipeline, facilitating swift deployment through Azure DevOps to support your SDLC/ALM processes that can be leveraged by your development and configuration teams and also aligns with the PBMM controls for configuration management. The repository also encompasses a versatile DocFX website which is an open source technical documentation markdown based framework created, maintained and leveraged by Microsoft for its own documentation and also the documentation site linked in the introduction.

### Dependencies

To ensure that all aspects of this theme are deployable in your environment, you will need the following:

1. **Dynamics 365 Customer Service Licensing (basic, pro, or enterprise)** - Minimum.
2. The unmanaged solutions contained in the `Solutions/Unmanaged` folder.
3. The customer self-service portal application installed in the target Dataverse environment.
4. If you are integrating with **Sign In Canada (TBS)** or **GCCF Consolidator (SSC)** and want to implement an enterprise SSO to centralize identities and control the profile and app usage in your organization, you will need to engage with either TBS or SSC for onboarding. This repository includes B2C configurations, and the Power Pages site supports both of these identity providers out of the box using the OIDC authentication profile functionality. Refer to [TBS Sign In Canada Service](https://connect.canada.ca/en/index.html) and [Canada's GCCF SSO service](https://sc-gc-pch.fjgc-gccf.gc.ca/GCCFAccess/FAQ.aspx). For the GCCF Consolidator service, please contact the SSC help desk to obtain your OIDC Client ID, Secret (or Private Key), Certificates (optional), and onboarding documentation.
5. Included in the theme is GCnotify integration, serving as an email or SMS platform in addition to the out-of-the-box email (server-side sync) for every Department or Agency (Federal). [Register your application](https://notification.canada.ca/) to get your API keys and develop your email templates.
6. An **Azure Subscription** is required to host supporting resources: B2C, Storage, Front Door, Log Analytics Workspace (optional), Sentinel (optional), Azure Monitor, KeyVaults, Custom Agent for DevOps (optional), Diagnostics, and billing policies for both the Dataverse environments and your DevOps organization (both optional).

### Overall Software/Cloud Architecture (Example)

![Software Architecture - Draft (Subject to minor updates)](Documentation/images/SDD/d2b7fb22cf85599b4a99ae89b6fe8ac6.png)

### Example Process (Development Team)

You will be provided with a branch to clone from the `yourproject` GIT repository locally using your preferred code editor, though we recommend Visual Studio Code with Power Platform build tools installed (CLI).

1. The cloned version will be "linked" to your Dataverse development environment, serving as an isolated workspace.
2. When ready to merge your changes into the primary development environment, initiate a pull request (PR) to the "Development" branch. This action triggers a pipeline to import your patch and portal code into the main development environment (unmanaged), allowing you to verify against dependencies and conventions.
3. Note that at 3 am daily, a pipeline migrates your specific patch to the staging (release) environment. However, manual triggering via the PowerPlatform-CI-Staging pipeline is available for validation in a managed environment prior to a full release (typically twice a sprint or more frequently).
4. During the issuance of releases to downstream environments like QA, UAT, CUT, PREPROD, and PROD, your participation is required for smoke testing your contributions.

### Process (Tech Leads)

You will receive a dedicated branch to clone from the `yourproject` GIT repository locally, using your preferred code editor. While any editor works, we recommend Visual Studio Code with Power Platform build tools installed (CLI).

1. The cloned version will be "linked" to your Dataverse development environment, providing an isolated workspace for your work.
2. When ready to merge your changes into the primary development environment, initiate a pull request (PR) to the "Development" branch. This action triggers a pipeline to import your patch and portal code into the main development environment (unmanaged), enabling you to verify dependencies and conventions.
3. Note that at 3 am daily, a pipeline migrates your specific patch to the staging (release) environment. You also have the flexibility to manually trigger this by running the PowerPlatform-CI-Staging pipeline. This manual trigger is vital to validate that your work is deployable in a managed environment, in preparation for a full release (typically twice a sprint, and sometimes more frequently).
4. During each sprint, every (X) day, you will issue a PR to the QA branch. This triggers a pipeline to clone yourproject and ORGANIZATION-Processes solutions into the main development environment, deploying them to Staging (upgrade). The generated artifact is saved for subsequent deployments.
5. If step 4 is successful, issue a PR to the UAT environment. This triggers a "release" pipeline, deploying the artifacts generated in step 4 to UAT. This is executed through the releases menu, enabling you to choose which release artifacts to deploy. The version in UAT is upgraded as well (managed).
6. If step 5 succeeds, new patches are provisioned in the main development environment. Developers must execute step 1 (clone or pull the assigned branch with the newly created patch assigned to them).
7. If step 4 is unsuccessful, the owner of the patch/portal PR is responsible for resolving the deployment issue. They can either issue a manual PR or wait for the process to run at 3 am. In this scenario, step 5 is blocked for that person's release artifacts only.
8. If step 6 is unsuccessful, the owner of the patch/portal PR is responsible for resolving the deployment issue. They can either issue a manual PR or wait for the process to run at 3 am. In this scenario, step 6 is blocked for that person's release artifacts only.
9. After UAT approval based on the deployed release artifacts, issue a PR to the PREPROD branch. This deploys all the tested artifacts.
10. Once step 9 is complete, conduct smoke testing (automated and manual). (*More information to follow on test plans, OWASP, etc.*)
11. If step 10 is successful, indicating the test suite passed, issue a PR to the "Main" branch. This deploys the artifacts to production. By this point, developers and analysts might have tested various new features and deployed them to UAT, but this process chooses the release artifacts within the scope of the UAT cycle for this particular release. Deployment is a continuous process, with the only minor "freeze" during step 4, which represents the official "release dates" defined within a sprint. Otherwise, releases are carried out daily to staging and QA, and twice a sprint (or occasionally less frequently, based on priorities) to UAT. PREPROD mirrors production, with the only substantial difference occurring during an actual release (step 9).

### Cloning the repository to your local terminal/git client/vs code

#### Step 1: Set up a Personal Access Token (repeat this step if your PAT expires)

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

#### Step 2: Re-authenticating your terminal/git client for subsequent Pulls and Clones when session expires and PAT token is configured (step 1)

1. At the root of the yourproject repo press the Clone button
2. In the left modal popup, press generate git credentials
3. In your VS Code, or Terminal (or which ever git client you are using), enter the following:

```console
git clone https://YOURUSERNAME:PASSWORDGENERATED@dev.azure.com/YourOrganization/YourProject/_git/YourRepo
```

### Release Pipeline Automation, CICD and QA

**Make sure you are comfortable with GIT, cloning the repository and issuig PR's to merge your local branch into the Main branch to update the repository before executing the pipeline**
In this diagram, the Developer provides the Dynamics 365 Solution Name, Data File, and Power Apps Portal to the DevOps Pipeline. The DevOps Pipeline stores the artifacts in the release for the Dev Environment, commits them to the GIT Repository, and stores the releasable artifacts in the Azure Artifact Storage.
The Release Manager issues the release to QA, UAT, PREPROD, and Production environments. The QA Team confirms the release for each environment. If the release is rejected, the QA Team notifies the Developer in the comment thread. The Developer then issues a new release which follows the same process as the initial release.


#### Minor Pipeline Sequence (Patches)-Developers only, and they are accountable to validate their releases to staging

![Subject to minor updates](Documentation/images/SDD/ebd835c6e9ddb602a745874fdc45a1f5.png)

#### Major Pipeline Sequence (Solution Clone - Full) - Tech Lead Accountability

![Subject to minor updates](Documentation/images/SDD/2f21a350a12d214885d9dea998486833.png)

### Environment map

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

### Wizard Form Configuration architecture

![Subject to minor updates](Documentation/images/SDD/ec0585993640016494873593500476ec.png)

### Enterprise Grants & Contributions

*Details coming soon* - baseline data model solution (EGCS-DS) is available under the Solutions/Unmanged folder. The unpacked and packed (managed) versions coming with the documentation update in our next release. A second version of the customer-self-service portal for G&C will be published as well in our next release slated for August 30 - which includes G&C specific baseline functionality whereas the customer-self-service includes the functionality for any dataverse environment with the D365 Customer Service (basic and pro) licensing available.
<!--
### Theme Preview and Example Implementations

#### Theme

#### International Scholarships (Global Affairs Canada)

#### Regulatory Compliance Case Management (Financial Sector) - FINTRAC -->
