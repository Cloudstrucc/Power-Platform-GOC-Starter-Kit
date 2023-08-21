# Azure Storage - Diagnostics & Technical Documentation & User Guides
[Download PDF](./Storage-Diagnostics.pdf)

## Pre-Requisites

-   **Dataverse environment administrator including user with PowerPlatform Administrator rights and System Administrator** security role assigned / access to the environment for which diagnostics is being configured.
-   **Global Administrator, Owner** of an Azure subscription hosting the Azure Storage resource

## Purpose

PowerApps Portals has a feature that will send runtime exception details and other errors to a series of log files that can be hosted in an Azure Storage Blob. This is useful to avoid displaying detailed trace logs to portal users and provides portal developers / administrators with the ability to review a detailed and verbose log of any exception / error thrown by the portal.

## Azure Storage & Blob Address

Navigate to the subscription that will host the Azure Storage resource and add a new resource. Search for Azure Storage (or Storage Account)

![A screenshot of a computer Description automatically generated with medium confidence](../images/Azure-Storage/b33f0f55b36dae62c9608df5bf3ff5da.png)

![Graphical user interface Description automatically generated](../images/Azure-Storage/7d3f3717b5eefc463580b40dfb55da3e.png)

Set the appropriate resource group and set the following parameters and go through the entire wizard and leave the defaults. These diagnostics are for development only.

![Graphical user interface, text, application Description automatically generated](../images/Azure-Storage/da062d7e9086aebe2a41af08fbe7ed91.png)

Once created, go to the newly created Azure Storage resource, click on the Access Keys menu and provide both the Key and Connection string to the PowerApps Portal developer / admin who is configuring the Portal settings to send the diagnostics log to the newly created blob.

![Graphical user interface, text, application Description automatically generated](../images/Azure-Storage/e12e79588a2e1f73b70606e9cc2f6e22.png)

Next, provide Reader access to the PowerApps Portal developer/admin(s) who will need to inspect the logs

![A screenshot of a computer Description automatically generated with medium confidence](../images/Azure-Storage/a7ac3940a2596899bb4cf46d839886c6.png)

![A screenshot of a computer Description automatically generated with medium confidence](../images/Azure-Storage/df55bc318118760f1105426f811a0733.png)

![Graphical user interface, application Description automatically generated](../images/Azure-Storage/b59669203e3a3772afa4f042e490ae1b.png)

## PowerApps Portal – Configure Diagnostics

*To complete the steps below you will need a connection string and key from the Azure Storage Resource created in the previous section.*

Navigate to <https://make.powerapps.com> and select the environment where diagnostics will be configured. Once selected, navigate to apps, click on the ellipsis next to the portal app, press “settings” and then “administration”

![Graphical user interface, application, Teams Description automatically generated](../images/Azure-Storage/a12c6bfb26ae84fa600cd9ff8b49d493.png)

In the portal administration console, click on Portal Actions and select “Enable Diagnostic Logging”

![Graphical user interface, calendar Description automatically generated with medium confidence](../images/Azure-Storage/757f56111b6bcbacaadc27ed56853495.png)

Paste the Connection String from the Storage Account and press configure

![Graphical user interface, application, Teams Description automatically generated](../images/Azure-Storage/8620b791d937ac1ec0a448771631a6a4.png)

Once completed, all portal diagnostics will be stored in the Azure Storage in a blob container and logs will be stored in folders by Date and the Portal’s App Registration ID. To view logs, go the Azure Storage resource, click on Containers and “telemetry-logs”.

![A screenshot of a computer Description automatically generated with medium confidence](../images/Azure-Storage/9f7a1c39a29366ba082c77647b70937a.png)

Each portal configured to send its diagnostics to this container will have a dedicated folder with there respective App Registration Record ID’s

![A screenshot of a computer Description automatically generated with medium confidence](../images/Azure-Storage/06e71083444da4618398bfb9b149799a.png)

Logs within the portal folder have a folder for each day and a file for each diagnostic sent to the container

![A screenshot of a computer Description automatically generated with medium confidence](../images/Azure-Storage/09b3e8e03a55bd3eae6377a122f8ad72.png)









