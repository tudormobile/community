# community - Community Apps and Services  
[![Build and Test Service (main)](https://github.com/tudormobile/community/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/tudormobile/community/actions/workflows/dotnet.yml)
[![Build Web App (main)](https://github.com/tudormobile/community/actions/workflows/web.yml/badge.svg?branch=main)](https://github.com/tudormobile/community/actions/workflows/web.yml)
![GitHub Issues or Pull Requests](https://img.shields.io/github/issues/tudormobile/community)  
![GitHub Service Release](https://img.shields.io/github/v/release/tudormobile/community?filter=%21*web*&label=Service%20Release)
![GitHub Web Release](https://img.shields.io/github/v/release/tudormobile/community?filter=*web*&label=Web%20Release)
[![License](https://img.shields.io/github/license/tudormobile/Community)](LICENSE)  
**Community** is a collection of applications and services for developing local community based applications.   

COPYRIGHT (C) BILL TUDOR

## Quick Start
### Web App
```sh
git clone https://github.com/tudormobile/community.git
cd web/community-web
npm install

npm run dev
```
### Web Service
```cs
using Tudormobile.CommunityService;
using Tudormobile.Dbx;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbx();
// ...
var app = builder.Build();
app.UseCommunityService();
// ...
app.Run();
```
Web Service Endpoints
- api.tudormobile.com
    - /[ category ]/[ service ]/[ .. ]

```
api.tudormobile.com
    /api
    /traffic
    /transit
    /weather
    /map/
        /community/v1 -> community service
        /community/v1/dbx -> dbx service
```
## Documentation
Community contains tools, services, and web apps for developing community based software solutions.

* [Tools](docs/tools.md)
* [Services](docs/service.md)
* [Web Apps](docs/web.md)

