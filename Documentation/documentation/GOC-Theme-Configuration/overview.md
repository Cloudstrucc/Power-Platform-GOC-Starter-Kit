# Configuration of the Enterprise Portal Theme (Power Platform)

[Download PDF](./overview.pdf)

## Pre-Requisites

- **PowerApps/B2C App Registration**: You will need to request a **ClientID** and **Secret** to {POWER PLATFORM ADMINISTRATOR MAILBOX TBD – CANVAS APP – COE KIT}
- **PowerApps Theme Deployment**: Request a ClientID and Secret to Download the latest version of the GAC PowerApps Theme (WET)
- **System Administrator** security role assigned to App Registration User in New Dynamics/PowerApps instance requiring the portal. This user will be
- **Azure PIM roles activated:**  Active Assignments of the following roles applied to user account creating the new Dataverse environment hosting the newly created Portal: PowerPlatform Administrator, Dynamics 365 Administrator, Application Administrator.
- To test the theme go to: <https://goc-theme-release.powerappsportals.com>

## PowerApps Portal Theme Deployment

Navigate to the <https://admin.powerplatform.microsoft.com> and In the environments menu press “New Environment”. Fill in a title (format should be EN(Acronym)-FR(Acronym)-Dev(Environment Type), description and ensure to select custom URL to ensure that the subdomain is not random and that you’ve selected English and \$CAD as the default language and currency respectively. Since the {ORGANIZATION} Portal Theme relies on the Dynamics 365 Customer Service App to be installed, ensure to check “Enable Dynamics Apps” and select customer service pro. \*Optional (but recommended) \* - {ORGANIZATION} should assign an Azure AD security group to govern access to any environment. This can be applied later, however if one is provided beforehand, make sure to set the Security group in this wizard. This environment provisioning can take up to 30 minutes.

![Graphical user interface, table Description automatically generated](../images/GOCThemeDeploy/327adb61b765d8908bf0e4bbe304a2d0.png)

![Graphical user interface, table, Teams Description automatically generated](../images/GOCThemeDeploy/17e48572eb0248412a53d2ceafa85f3a.png)

![Graphical user interface, text, application, chat or text message Description automatically generated](../images/GOCThemeDeploy/a110720844fcb82e67e18936ac8a3839.png)

Once the environment has been provisioned, navigate to the <https://make.powerapps.com> and select the newly created environment. In the side panel, select “Create” and once the application library is rendered, select “Customer self-service”. The follow the convention, it is recommended that the portal title and subdomain matches the title of the environment. The portal provisioning process can take up to 20 minutes.

![Graphical user interface, application, Teams Description automatically generated](../images/GOCThemeDeploy/f1d33f4af2ae8c10a095e480a5ddd468.png)

Once provisioned, navigate to the portal application to ensure its running

![Graphical user interface, application, Teams Description automatically generated](../images/GOCThemeDeploy/143cabc55fc83fe3d461451220d756be.png)

![A screenshot of a video game Description automatically generated with medium confidence](../images/GOCThemeDeploy/0f33e03a984fd36c3f547c873cfaf039.png)

Delete the newly created portal application in PowerApps as this will be replaced with the Enterprise Theme. The portal installation is required to ensure the environment has the necessary solutions and web application configured.

![Graphical user interface, application Description automatically generated](../images/GOCThemeDeploy/983f48324ab6141cda53d03980dbce07.png)

Go to the Portal Management Model Driven App and Delete the website record

![Graphical user interface, text, application Description automatically generated](../images/GOCThemeDeploy/2bc69dc9d23ce30ec856b10f17bfd193.png)

Navigate to the Dynamics 365 System Settings and temporarily remove file attachment and file size (adjust) restrictions. This ensures that the theme’s JavaScript files can be uploaded and not blocked by the API. Make sure to copy the file restrictions to your notepad or elsewhere as once the theme has been uploaded, you will need to add these restrictions back.

![Graphical user interface, application Description automatically generated](../images/GOCThemeDeploy/40a0aa1de6107b7643dfa2b88724d85c.png)

![Graphical user interface, application Description automatically generated](../images/GOCThemeDeploy/e95f43b63a40b0109969eba51e0623db.png)

![Graphical user interface, application Description automatically generated](../images/GOCThemeDeploy/de4af876682d57a4bca064cfc73c499a.png)

![Graphical user interface, application, Teams Description automatically generated](../images/GOCThemeDeploy/990a2e3b91f750d26eaf47396096384c.png)

![Graphical user interface, text, application, email Description automatically generated](../images/GOCThemeDeploy/88de67ab5df1e25a956241a407d46f1a.png)

![Graphical user interface, text, application, email Description automatically generated](../images/GOCThemeDeploy/9057a5e8aa616d266df8139d9bac4437.png)

Next install the French Language Pack by going back to the System’s administration settings and selecting Languages, and then French (1036)

![Graphical user interface, application, Teams Description automatically generated](../images/GOCThemeDeploy/9c669b96bdff4d67ef1c62b28bf2a1d7.png)

![Graphical user interface, application Description automatically generated](../images/GOCThemeDeploy/8c901726ffdab2a1be56280988ce3112.png)

## Deploying the theme

To deploy the {ORGANIZATION} Enterprise Theme, download the portal CLI extension in Visual Studio Code. Once downloaded, create a new project in an empty directory on your computer. Next, once the empty project is opened in your IDE, connect and download the {ORGANIZATION} Portal Theme – Release environment by using the commands below.

Before running these commands, download the portal from the following GIT repository:

<https://github.com/Cloudstrucc/PowerApps-WET-Canada> (e.g download the files, or clone the repository)

```
-   pac auth create --url "https://{ORGANIZATION}-portal-theme-dev.crm3.dynamics.com" --applicationId "cbe003cd-ecfe-4324-84cb" --tenant "b644-288b930b912b" --clientSecret "tzm7Q\~ " **\<-Connect to theme environment**
-   pac paportal upload -p .\\customer-self-service\\ **\<-Folder of downloaded portal from GITHUB**
```

Once the upload is completed, return to make.powerapps.com, select your environment, and create a new portal application and make sure to check “use existing website record” and select “customer-self-service”. Allow the Dataverse to finalize the deployment for 10 minutes. Once the 10 minutes has elapsed, you should be able to navigate to the portal and the Canada.ca theme will render.

Convert the Portal from Trial to Production. Go the portals admin console and in the Portal details menu select Convert and confirm. Before doing so locate the App Registration user in the Azure Portal and yourself (and other administrators of this environment) as Owner. Note you will need to have the Application Administrator Azure Role in your active assignments.

![A screenshot of a computer Description automatically generated](../images/GOCThemeDeploy/f817fef3e6a153eb7b61b558b6e21c63.png)

![Graphical user interface, text, application Description automatically generated](../images/GOCThemeDeploy/6c8c64d7e0cbc98d9485191231a52aa8.png)

Validate you are working with the correct App Registration user by navigating to the Authentication blade and inspecting the Redirect URLs to ensure it matches your newly created environment.

![Graphical user interface, text, application Description automatically generated](../images/GOCThemeDeploy/6c8c64d7e0cbc98d9485191231a52aa8.png)

![Graphical user interface, text, application, Teams Description automatically generated](../images/GOCThemeDeploy/ab704aad858ef1500896f6f6345a7c4f.png)

![Graphical user interface, application, Teams Description automatically generated](../images/GOCThemeDeploy/12306f096b1c767153bb91278872c950.png)

![Graphical user interface, application Description automatically generated](../images/GOCThemeDeploy/a26df0fd3bc0d9ba969175f9162bd1bd.png)

![Graphical user interface, application, Teams Description automatically generated](../images/GOCThemeDeploy/ac22dbcf22555e6291438bcea21a6bc3.png)

Add the PowerPlatform-CICD App Registration user to the newly created environment. Go to the environment in the PowerPlatform admin centre and select Users on the right and follow the rest of the steps below.

![Graphical user interface, text, application, email Description automatically generated](../images/GOCThemeDeploy/ff3a2ccbcd5632ddaec6b2163a2364c7.png)

![Graphical user interface, application Description automatically generated](../images/GOCThemeDeploy/1da091e95899937da4fe31d7caf5e52b.png)

![Graphical user interface, application, Teams Description automatically generated](../images/GOCThemeDeploy/661960549d8769519d4ff136833249a1.png)

![Graphical user interface, application Description automatically generated](../images/GOCThemeDeploy/2f1c44331a61ee0f72701faae9ef064d.png)

![Graphical user interface, application Description automatically generated](../images/GOCThemeDeploy/834fb101a83f1d119bc88d47b4844348.png)

![Graphical user interface, application Description automatically generated](../images/GOCThemeDeploy/f752c1d6ad9d4937f4b8d6f18a36099d.png)

![Graphical user interface, application Description automatically generated](../images/GOCThemeDeploy/234365b195f5239c748153b68d6c7945.png)

Restart the portal

![Graphical user interface Description automatically generated](../images/GOCThemeDeploy/2a08aac71ceabdbe8d74cb1d305eb24e.png)

If the portal is still rendering the original theme, you can verify and set the binding to point to the theme’s binding.

![Graphical user interface, application Description automatically generated](../images/GOCThemeDeploy/fcfc0b6f24cf4e96ac4e30a4875201f0.png)

Once uploaded, go to portal’s admin centre, and restart. Once restarted, the portal will should render the enterprise theme.

![Graphical user interface, application Description automatically generated](../images/GOCThemeDeploy/749882836967e9e92ed178e094cd263c.png)

Now that the theme is installed, edit the authentication setting entitled “Enterprise SSO” and enter in the ClientID, Secret, and the redirect URL’s domains to match the newly created portal’s domain and restart the portal. \*NOTE you will need to send your portal domain to the Azure B2C administrator who will provide you with a ClientID and Secret to replace.

![Graphical user interface, application, Teams Description automatically generated](../images/GOCThemeDeploy/a495613960521e921ba16807d21364d6.png)

![Graphical user interface, application Description automatically generated](../images/GOCThemeDeploy/5e29a3b3685d683b7f8baed0dfc6b824.png)

(Optional Step) Set the IP Restrictions to the Portal to only allow network flows from the {ORGANIZATION} Network (VPN or Direct). The IP(s) must be in CIDR notation. You can reference the “{ORGANIZATION}-portal-theme-dev” deployment to obtain the IP restriction list.

![A picture containing graphical user interface Description automatically generated](../images/GOCThemeDeploy/10bb8a4eb9b3a3fac1ab24a83c82e159.png)

## Azure B2C – Configuring a new Client

As the Azure B2C ({ORGANIZATION}-gccf-dev domain) Administrator, navigate to the Azure B2C tenant and follow the steps below to register an application to leverage the enterprise SSO. Once completed, provide the ClientID to the PowerApps Portals administrator to set the value in the PowerApps Portals site settings. You will need to receive the portal domain from the PowerApps administrator before completing the steps below.

![Graphical user interface, text, application, email Description automatically generated](../images/GOCThemeDeploy/eac8ca6ba83af7fe80a6037ad75ec05b.png)

![Graphical user interface, text, application, email Description automatically generated](../images/GOCThemeDeploy/e6a91865d8a58aea79dc648f632e56f8.png)

![Graphical user interface, text, application, email Description automatically generated](../images/GOCThemeDeploy/33866d9d685113db97e2fdae60390bfc.png)

Provide the Application (client) ID to the PowerApps administrator

![Graphical user interface, text, application Description automatically generated](../images/GOCThemeDeploy/6340961daec54646c9f4dffcf45ab5fb.png)
