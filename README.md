# Event Manager Service
 The project repository aims to manage SaaS events from SaaS providers for ease of decoupled SaaS event testing.
 
 Below shows a simple architecture diagram to listen SaaS events from SaaS providers(e.g. One Drive, Box, OAuth, etc):
 ![alt text](/docs/architecture.PNG)
 
__Steps involved:__ 
 1) Client service subscribes to SaaS Service with POST Event Manager Service callback url with Notification ID which is a unique representation for subscription.
 ```
 Example:
 {
    "callbackUrl" : "https://<hostname>/event/api/callback/<Notification ID>?key=<Access Key>"
 }
 ```
2) Client service starts polling for events to Event Manager Service with GET Endpoint.
 ```
 Example:
 GET https://<hostname>/event/api/callback/<Notification ID>?key=<Access Key>"
 ```
 3) SaaS Service received events from the desired event.
 4) SaaS Service will callback with the given POST Event Manager Service callback url.
 ```
 Example:
 {
    "callbackUrl" : "https://<hostname>/event/api/callback/<Notification ID>?key=<Access Key>"
 }
 ```
 5) Event Manager Service receives the SaaS event and adds into In-Memory Cache(expiry of 10 minutes) for usages.
 6) Client services will receive the SaaS event during the polling.
