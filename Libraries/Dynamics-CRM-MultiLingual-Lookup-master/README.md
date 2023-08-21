# Dynamics CRM MultiLingual Lookup

## Installation

### Pre-requisite:

Additional language packs must be enabled. To enable the language pack, go to Settings / Administration / Languages. Check "Target Language" / Close.

Import MultiLingualLookup_1_0_0_2 solution. If unmanaged solution is imported, click "Publish All customization" button once you import the solution. and then "Close".

## Configuration

### Pre-requisite:

To use the multi-lingual feature for a field in your entity, you must create additional fields for each language:

### Example for Account entity:

**First Field**

| Primary Field Name | Language1 Field Name | Language2 Field Name |
| --- | --- | --- |
| name (OOB) | new_name_en | new_name_fr |

**Second Field**

| Primary Field Name | Language1 Field Name | Language2 Field Name |
| --- | --- | --- |
| new_secondname | new_secondname_en | new_secondname_fr |

Notes:
1. The maximum length of the primary field must be double + additional 10 characters to the English and French field2.	s.
1. The primary field must be hidden. In Form editor / go to field properties for the primary field (by double-clicking on the form field) and uncheck "Visible by default". Save the form and publish.

### Create Configuration

<p><img src="https://github.com/mehrgithub/Dynamics-CRM-MultiLingual-Lookup/raw/master/MultiLingual.PNG" /></p>

1. Open Advanced Find.
1. From the “Look for” drop down, select "Multi Lingual Lookup Configuration".
1. Click on the Results ribbon button.
1. In the results section, click on the New Multi Lingual Lookup Configuration ribbon button.

### Configuration format:

**Primary Entity** =  Schema name of the muli-lingual entity

**Primary Fields** =   Primary field schema name[Language1 field schema name;Language1 LCID|anguage2 field schema name;Language2 LCID]

**Related Entities and Fields**  =  Related entity1 schema name|lookup1 schema name|lookup2 schema name,Related entity2 schema name|lookup1 schema name

Note:
For Primary Fields and Related Entities, use “,” character to indicate multiple fields

### Example for Account entity:

**Primary Entity** =    account

**Primary Fields** =    name[new_nameen;1033|new_namefr;1036],new_name2[new_name2en;1033|new_name2fr;1036]

**Related Entities and Fields** =   contact|parentcustomerid|parentcustomerid1,account|parentaccountid

**Note:**

"|" character is used for concatenating language specific labels. Do not use the "|" characters in the Primary Fields labels.

<p>Feel free to <a href="https://www.paypal.me/MehrdadHatteffi">donate(paypal)</a> if this saved you some time or helped out :)</p>
