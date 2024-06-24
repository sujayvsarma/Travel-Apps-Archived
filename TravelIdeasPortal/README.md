# TravelIdeasPortal
The v1.0 version of the travel ideas portal (formerly "fly2.in"), where we have better integrations, more APIs and richer UX. Hopefully, we are also more mobile friendly.
---

## Dependencies
Clone this repo as follows to get everything:

```
$ git clone --recurse-submodules https://github.com/sujayvsarma/TravelIdeasPortal
```

Note that you will get two instances of the RestApi -- once in the main directory, and again within the Wikipedia directory. This is fine. Our portal project directly uses the outer RestApi SDK, while the Wikipedia SDK uses the one within its submodules directory. To avoid having multiple versions of the DLL and getting into ".NET DLL Hell", ensure that Wikipedia SDK is always the latest. To clarify

Project|Depends on
-------|-------------
SujaySarma.Sdk.RestApi|None
SujaySarma.Sdk.WikipediaApi|SujaySarma.Sdk.RestApi (SujaySarma.Sdk.WikipediaApi/submodules/SujaySarma.Sdk.RestApi)
TravelIdeasPortal (**the main project**)|SujaySarma.Sdk.RestApi and SujaySarma.Sdk.WikipediaApi (both in the outer-most folder)

---


