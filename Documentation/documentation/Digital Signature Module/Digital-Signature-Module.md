
# Digital Signatures - Introduction (PHASE 2)

[Download PDF](./Digital-Signature-Module.pdf)

A systematic approach of ensuring data integrity using public key encryption techniques.

## Design Principles

- Immutability
- Auditabilty / monitoring
- Strong cryptography
- Platform agnostic (API Driven)
- Integration with Active Directory & Entrust (GoC/{ORGANIZATION} standards)
- MFA as a pre-cursor to signature
- Multi layered access (access tiers) / administration (separate RBAC model/separation of concern)
- Traceability
- Data residency & ownership
- Abstractions / transparent to user (e.g., low complexity UX and configuration)

## Architecture & Software (diagram to-do)

- Azure Immutable Storage
  - Persistent layer storing JSON payload re
- Azure KeyVaults
  - Leveraged for signing, encryption, de-cryption for digital signature
   payloads stored in both Dataverse & Immutable storage
  
- MS Authenticator
- Entrust PKI
  - Private/Pub key pairs (or keys) – integration with AKV?
- D365 SDK/API
  - Class libraries (plugins) + odata endpoint to interface with
   Immutable Storage & AKV.
- Graph API
  - Azure API platform to interface with azure services (leverage App
   Registrations for authorization to AKV, Immutable storage)

- Log Analytics
  - Log platform that will ingest / aggregate all logs for the digital
   signatures module

## Immutable Record Policies & Applicability of these policies

> **Time-based retention policies**
With a time-based retention policy,
    users can set policies to store data for a specified interval. When a
    time-based retention policy is set, objects can be created and read,
    but not modified or deleted. After the retention period has expired,
    objects can be deleted but not overwritten.

> **Legal hold policies:**
> A legal hold stores immutable data until the legal hold is explicitly cleared. When a legal hold is set, objects can be created and read, but not modified or deleted.

>**Regulatory compliance:**
>Immutable storage for Azure Blob Storage will address SEC 17a-4(f), CFTC 1.31(d), FINRA, and other regulations. ***(equivalent GoC specific policies to be added here + algorithms)***

>**Secure document retention:**
>Immutable storage for blobs ensures that data can't be modified or deleted by any user, **not even by users with account administrative privileges.**

![enter image description here](../images/Digital%20Signatures%20Module/Picture1.png)

## Application Architecture / Key Features

- Decision module includes flag for digital signing. If true,
   administrator will select a configured “digital signature
   configuration”
  - Digital signature configuration administration module    includes:
    - Payload configuration: Tables/Columns -> relationships to include in a generated JSON object
    - Security Role(s) / Team(s) permitted to generate payload
- Signature: When a user creates the decision, they are challenged for MFA (authenticator/TOTP) and once successful, are prompted to provide their private key which is verified against the corresponding public key in Azure KeyVaults. In addition.
- A successfully signed decision, will construct a JSON object of the associated record by referencing the payload schema of the “digital signature configuration” and cryptographically sign it using the users’ private key and store the record in Azure Immutable storage.
- The address of the Azure Immutable storage record that holds the record will be stored in the CRM decision and on the record in question (e.g. a security clearance).
- In addition, once the user signs the record, the same encrypted / hashed version of the JSON payload is stored in the “core_hash” multiline text field in CRM, and a CRM plugin that runs on retrieve, update, delete, append, and append to will reference this value and compare it against the immutable value for any discrepancies and will prevent any further updates on the CRM side.
- In addition, Azure Sentinel and the Compliance Centre will monitor that the public facing (or mutable) version of the official record matches the the immutable value (querying both the Dataverse API & Storage API).
- Dashboard of the aggregation of results to be designed, events configured (alerts), and process for actioning discrepancies.
- Ability to generate a physical copy of the official record is an RFID (or similar) representation of the payload (TBD/Optional)

## Network Security

> Traffic must originate from a VNet. A VNet enables clients to securely connect to your storage account. The only way to secure the data in your account is by using a VNet and other network security settings. Any other tool used to secure data including account key authorization, Azure Active Directory (AD) security, and access control lists (ACLs) are not yet supported in accounts that have the NFS 3.0 protocol support enabled on them.

| Setting | Comments |
|--|--|
|Configure the minimum required version of Transport Layer Security (TLS) for a storage account.  | Require that clients use a more secure version of TLS to make requests against an Azure Storage account by configuring the minimum version of TLS for that account. For more information, see [Configure minimum required version of Transport Layer Security (TLS) for a storage account](https://docs.microsoft.com/en-us/azure/storage/common/transport-layer-security-configure-minimum-version?toc=/azure/storage/blobs/toc.json) |
|Enable the Secure transfer required option on all of your storage accounts|When you enable the Secure transfer required option, all requests made against the storage account must take place over secure connections. Any requests made over HTTP will fail. For more information, see [Require secure transfer in Azure Storage](https://docs.microsoft.com/en-us/azure/storage/common/storage-require-secure-transfer?toc=/azure/storage/blobs/toc.json).|
|Enable firewall rules|Configure firewall rules to limit access to your storage account to requests that originate from specified IP addresses or ranges, or from a list of subnets in an Azure Virtual Network (VNet). For more information about configuring firewall rules, see [Configure Azure Storage firewalls and virtual networks](https://docs.microsoft.com/en-us/azure/storage/common/storage-network-security?toc=/azure/storage/blobs/toc.json).|
|Allow trusted Microsoft services to access the storage account|Turning on firewall rules for your storage account blocks incoming requests for data by default, unless the requests originate from a service operating within an Azure Virtual Network (VNet) or from allowed public IP addresses. Requests that are blocked include those from other Azure services, from the Azure portal, from logging and metrics services, and so on. You can permit requests from other Azure services by adding an exception to allow trusted Microsoft services to access the storage account. For more information about adding an exception for trusted Microsoft services, see [Configure Azure Storage firewalls and virtual networks](https://docs.microsoft.com/en-us/azure/storage/common/storage-network-security?toc=/azure/storage/blobs/toc.json).|
|Use private endpoints|A private endpoint assigns a private IP address from your Azure Virtual Network (VNet) to the storage account. It secures all traffic between your VNet and the storage account over a private link. For more information about private endpoints, see [Connect privately to a storage account using Azure Private Endpoint](https://docs.microsoft.com/en-us/azure/private-link/tutorial-private-endpoint-storage-portal).|
|Use VNet service tags|A service tag represents a group of IP address prefixes from a given Azure service. Microsoft manages the address prefixes encompassed by the service tag and automatically updates the service tag as addresses change. For more information about service tags supported by Azure Storage, see [Azure service tags overview](https://docs.microsoft.com/en-us/azure/virtual-network/service-tags-overview). For a tutorial that shows how to use service tags to create outbound network rules, see [Restrict access to PaaS resources](https://docs.microsoft.com/en-us/azure/virtual-network/tutorial-restrict-network-access-to-resources).|
|Limit network access to specific networks|Limiting network access to networks hosting clients requiring access reduces the exposure of your resources to network attacks.|
|Configure network routing preference|You can configure network routing preference for your Azure storage account to specify how network traffic is routed to your account from clients over the Internet using the Microsoft global network or Internet routing. For more information, see [Configure network routing preference for Azure Storage](https://docs.microsoft.com/en-us/azure/storage/common/network-routing-preference).|

##

## Acceptance Criteria Administrator

### Global configuration

- Can configure Azure immutable storage container address (CRM Settings) for environment (1:N)
  - If 1 or more blobs are linked, update not permitted – can create new
   container address record. Can set the active address set to Immutable
   storage address lookup N:1

### Digital Signature Type Configuration

- Can create digital signature type record
  - Can associate decision type(s) to digital record type record
  - By associating the decision type, the table that is associated in
   the decision configuration will automatically be chosen for the
   “digital signature record schema” multi line text field which holds
   the JSON representation of the table holding records that can be
   signed/encrypted with this module.
  - Can create/update schema to include association(s) to primary
   ”signable” record

- Can activate digital signature record
  - Co-administrator approves configuration (decision)
- Can archive digital signature type record
  - Co-administrator approves configuration (decision), transitions
   state to “archived”
- Can view / export report of digital signature type record usage
- Can view / export report of permitted users

## Acceptance Criteria: CRM Users (non-admin)

### Certificate configuration

- Can configure download encryption/signing key and install to Entrust CA store on organization machine (create w/ password - export w/ private key and import) – canvas (or desktop -> offline only)

### Digitally signing a record (via decision)

- Can provide approve MFA challenge upon creation of decision record
- Can provide private key pwd upon successful MFA response/approval
  - Decision record state transitions to “Completed” and “Signed by” set
   to user.
- Cannot delete or update signed record (and decision record that
   invoked the signature request). Update prevention includes
   associations configured in the digital signature type configuration
   record (schema for payload)
- Instead of edit, must create a new decision record to revert state
- (optional) – email confirmation of digitally signed record includes
   link to signed record

### Data Architecture

![enter image description here](../images/Digital%20Signatures%20Module/Picture2.png)

### Digital Signature Type Configuration Example (in Dataverse)

Table: FINTRAC_riskassessment

 Columns: FINTRAC_contactid, fristname, lastname, email, effectivedate, … {all}

 Relationships: FINTRAC_contact, FINTRAC_organization

 Related Decision Types: Approved, Rejected, …

 Blob Container Address: {immutable_storage_connection_string}

 JSON Schema: {

 Auto generated on save based on values

 }

### JSON Payload example

#### Unencrypted values saved in the digital record

![Unencrypted JSON Payload](../images/Digital%20Signatures%20Module/Picture3a.png)

#### Encrypted values saved in the digital record

![encrypted object](../images/Digital%20Signatures%20Module/Picture3b.png)

## Digital Signature Journey

![enter image description here](../images/Digital%20Signatures%20Module/Picture4.png)









