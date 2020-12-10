# Tickr

Tickr is a __gRPC__ learning application. Three methods aree exposed through the service:

> The latest (beta) version of Insomnia supports testing gRCP services.



### GetAll Operation

Returns a stream of __TodoModel__ records stored in the RavenDB. The __gRPC__ contract can be set to return all Todo's or only the incomplete ones.

> Uses the `TodoFilterRequest` gRPC message.

```json
{
	"includeCompleted" : false
}
```
    
### Add Operation

Create a new Todo record in RavenDB. Creation date is defaulted and is not supplied in the message to create the item.

> Request are sent with a `TodoRequest` gRPC message.

```json
{
	"description" : "Test Add 2",
	"complete" : false
}
```

> Responds with a `TodoReply` message

```protobuf
message TodoReply {
    string id = 1;
    string description = 2;
    google.protobuf.Timestamp created = 3;
    bool complete = 4;
}
```




### Complete Operation

Marks a Todo item as complete. 

> Request are sent with a `CompleteRequest` gRPC message. 

```json
{
	"id" : "TodoModels/226-A"
}
```
> Responds with a `CompleteReply` message.
```protobuf
message CompleteReply {
    string id = 1;
    string status = 2;
}
```
___

## Using client secrets to configure the application. 

RavenDB configuration is defined in __appsettings.json__ under the __RavenSettings__ section. values in this section is left blank and should be added to the client secret store before running the application.

### Adding values to the user secrets store

> You must execute the commandline operations from the folder where your __csproj__ file is.

Set the _ServerUrl_ secret.
```bash
dotnet user-secret set "RavenSettings:ServerUrl" "https://your-url-here"
```


Set the _CertificationLocation_ secret.
```bash
dotnet user-secret set "RavenSettings:CertificateLocation" "<Path to certificate (PFX) file>"
```
> When using RavenDB cloud, the certificate is required login to the database. You can download the certificate from the RavenDB Studio.


Set the _Auth0 Domain_ secret.
```bash
dotnet user-secret set "Auth0:Domain" "xxx.auth0.com"
```
> This projects uses __Auth0__ for authorization. This value is used to set the name of the registered Auth0 domain.  

Set the _Auth0 Audience_ secret.
```bash
dotnet user-secret set "Auth0:Audience" "audience value"
```

Set the _Auth0 ClientId_ secret.
```bash
dotnet user-secret set "Auth0:ClientId" "clientId from Auth0"
```

Set the _Auth0 ClientSecret_ secret.
```bash
dotnet user-secret set "Auth0:ClientSecret" "clientsecret from Auth0"
```
> This projects uses __Auth0__ for authorization. This value is used to set the audience for the registered API in Auth0.  

