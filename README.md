-- 1. API Gateway --
Our initial system only contained a single API, responsible for handling requests from our (theorerical) user interface. An API gateway made little sense here, so we removed the previous API, and made our 3 "Services" into API's, and now having a Gateway is a lot more sensible. 

Practically, this can be seen in our "ApiGateway" project, where we implement the gateway using Ocelot (ocelot.json, program.cs)

-- 2. Reliability --
The key areas of failure in our system that we identified mostly pertained to networking and communication between our microservices. Specifically failure to communicate due to unavailability.

To address this, we have implemented Polly for the purpose of adding both retry and circuit breaker policies to our HttpClients responsible for microservice communication. The result of this is that our system is now more robust in case of temporary unavailability due to retries, and even if that fails, a circuit breaker is in place to ensure that the failure is as localized as possible. 

-- 3. Kubernetes --
We have created manifest files for our system in the folder "k8s" found in the root directory of this project. Of note, there is the RabbitMQ cluster, and we have a "PersistenVolumeClaim (pvc)" for our mssql database, which is responsible for data persistence so that our database doesn't just reset every restart or rescheduling, which would not be ideal for pretty obvious reasons. 

As for the deployment, attached to our Hand-in there will be a picture taken of our Kubernetes dashboard showing a successful deployment

-- 4. Security --
Overall, Kubernetes and Docker already provide several layers of security through isolation and orchestration, meaning that there is very little inherent risk in our microservice-to-microservice communications. We have had a few considerations, however:

a) DDos, Injection & Privileges
I am just going to merge these points together, as we right now don't have a defined UI with entry points and different kinds of users etc., but theoretically we don't really do much to limit our traffic (retries help a little), we do not scrutinize our inputs and we do not have any access levels, meaning everyone has access to everything. We are not going to remedy a lot here. To sort of remedy some of this, we are using JWT Bearer tokens through our AuthService, and are applying rate limiting to our ApiGateway in ocelot.json. Input sanitation I think is better done in the UI itself through e.g. masking, so we are leaving that as theoretical.

b) Secrets, appsettings etc.
We do not hide our connection strings, or really any sensitive information for that matter. For this, we are using Hashicorp to store our secrets so that they aren't just hard-coded into our solution where "anybody" could just read them

c) Exposed Ports
Our ports are exposed in our docker-compose, so to remedy this we remove our ports (except for ApiGateway) from our docker-compose and make them all run on the internal network we define in our docker-compose, 'appnetwork', instead. This makes it so, that the only way to call the services is through our ApiGateway port, where we used Ocelot to define specific routes for our other services.


-- 5. Design Patterns --
Design patterns we have researched and considered or implemented are:

a) Sidecar
We have implemented the Sidecar pattern in our application by way of Fluentd which we use for logging. Perhaps not the most interesting application, but still a good example of an implementation

b) Saga
Not implemented. Transactions that span our microservices seems really nice, though, but we got caught up in other stuff.
	
c) Event Sourcing
Not implemented. Frankly, it was pretty difficult to see how we would even implement it, even though the consistency it adds to our data seems nice
 
d) Api Gateway
This one we already implemented as part of the mandatory first task, so not much to say here.

e) Circuit Breaker
This one we have already implemented through Polly, as part of our retry processes. This pattern improves the reliability and resilience of our system, as it makes it less likely that we run into a scenario that is likely to cause a failure

f) 1 Database per service
This one we did not end up implementing, mostly due to time constraints. It would help to make our microservices even more independent of each other with very little downside
