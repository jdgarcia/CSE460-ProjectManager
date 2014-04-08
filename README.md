__WARNING__: We will probably get a lot of conflicts from the database file, so I want to .gitignore it once we have some sample data with all our services working.

#Project Structure

##Backend
The main files to be concerned with are those in the /Controllers folder.
The /Models folder has one LINQ to SQL class file that links all our database tables to model classes.

##Frontend
/Views is where our HTML pages will go, and is the main place where our back-end services will be consumed from.

##REST Services
The ASP.NET MVC project makes all our Controller functions RESTful services, so using the services is simple.
The API structure is /{ControllerName}/{OperationName}/{arguments}

Example: The form used to create a tenant is
```
<form action="/Tenant/Create" method="post">
    <label for="OrgName">Organization Name: </label>
    <input type="text" name="OrgName" />
    <input type="submit" value="Register Organization" />
</form>
```

Which calls the TenantController class' Create method, which has the signature
`public ActionResult Create(string OrgName)`
