# Event Manager Service [![Build Status](https://dev.azure.com/saibaskar57/saibaskar57/_apis/build/status/saibaskaran57.EventManager?branchName=master)](https://dev.azure.com/saibaskar57/saibaskar57/_build/latest?definitionId=1&branchName=master)
 The project repository aims to manage SaaS events from SaaS providers for ease of decoupled SaaS event testing.
 
 Below shows a simple architecture diagram to listen SaaS events from SaaS providers(e.g. One Drive, Box, OAuth, etc):
 ![alt text](/docs/architecture.PNG)
 
 __Key Identifiers:__
 ```
 <Notification ID> ~ represents the unique identifier to subscribe to SaaS Service and fundamentatal as the identifier will be used to poll for events.

 <Access Key> ~ represents the access key to Event Manager service for basic authentication.
 ```
__Steps involved:__ 
 
 1) Client service subscribes to SaaS Service with __POST__ Event Manager Service callback url.
 ```
 Example:
 {
    "callbackUrl" : "https://<hostname>/event/api/callback/<Notification ID>?key=<Access Key>"
 }
 ```
2) Client service starts polling for events to Event Manager Service via __GET__ Event Manager Service Endpoint.
 ```
 Example:
 GET "https://<hostname>/event/api/callback/<Notification ID>?key=<Access Key>"
 ```
 3) SaaS Service received events from the desired call to action.
 4) SaaS Service will callback with the given __POST__ Event Manager Service callback url.
 ```
 Example:
 {
    "callbackUrl" : "https://<hostname>/event/api/callback/<Notification ID>?key=<Access Key>"
 }
 ```
 5) Event Manager Service receives the SaaS event and adds into In-Memory Cache(entry expiry of 10 minutes) for usages.
 6) Client services will receive the SaaS event during the polling.
