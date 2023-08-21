# Portal commands

```console
//connect to source
pac auth create -u "https://goc-theme-release.crm3.dynamics.com";
//download portal
pac paportal download -id '7b138792-1090-45b6-9241-8f8d96d8c372' -p Portals/ -o
```

```console
//Connect to target
pac auth create -u "https://goc-theme-release.crm3.dynamics.com";
pac paportal upload -p Portals/customer-self-service/ -dp 'release'
```
