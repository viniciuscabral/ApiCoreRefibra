# ApiCoreRefibra

This API was created to serve to part of the ecosystem REFIBRA(Repositório Filatélico Brasileiro). It was concepted to be a bridge between a databese using Apache Fuseki and a frontend aplication that represent a site to research knoledgment in a differente way. I builded this API using languagens and tools that i work nowadays to way to be simple and replicatable on differents enviropments. Hence it was developed in .NET Core 2.1 (https://github.com/dotnet/core) using IDE Visual Studio Code.
Apache Jena Fuseki (https://jena.apache.org/documentation/fuseki2/) was used like a NoSql database to save triple RDF information and serve as point to SPARQL query over this triples RDF (https://en.wikipedia.org/wiki/Resource_Description_Framework). For us case the system manipulate te information inputed by user and process evething to a RDF triples using Wikifier API (http://wikifier.org/).

### Runtime

Afeter a brief information about the API is possible understand a litle bit the runtime and thing that it do.
Today is a incertain location yet but the software will be impove new function and ways throughout researche's development. 

![api](https://github.com/viniciuscabral/ApiCoreRefibra/blob/master/apisystem.png)


# Context

This project is son of a great research realise in the context Vinícius Cabral's Master degree in Federal University of Pernambuco (UFPE) under supervison of Dr. Diego Salcedo. Nowadays it stand in developmento until 2020 but has a Abstract like that:

This research it’s about a practical proposal for the application of Interactive Epistemography with the use of technological tools of the Semantic Web prototyped with Postal Stamps as informational items and the interacting agent as a source of information. The positivist way of classification leaves no space for knowledge outside the cycles of specialized domain to be disclosed and exploited, a hierarchical classification avoid other possibilities. In this way it is necessary to think of integrative practices that give equal opportunity so that different world views can be used to represent items in digital environments, an open and constantly changing classification. As a consequence of the insertion of logical pluralism the possibility of discovering different knowledge, in a research or navigation, is enhanced the encounter with other types of thinking: serendipity. In this sense, this research aims to discuss and develop within the context of the Brazilian Philatelic Repository (REFIBRA) a proposal for the application of Interactive Epistemography, based on concepts such as declassification and autonarration, using Semantic Web technologies and providing the participation of the interacting agent as constant resignification of informational items, in this scenario the Postal Stamps. Therefore, in the research was used the bibliographic and documentary survey to identify the discussion of concepts and basic practices for the development of the proposed application. Subsequently the project took practical steps to create the technological tool that would allow the concrete application of what was studied in the first stage. In this way it was possible to create a Web environment in which it is possible to participate in any world view in equal measure, besides providing the retrieval, meeting and discovery of information.

# How to use

Docker version 2.1+

1. git clone https://github.com/viniciuscabral/ApiCoreRefibra.git
2. cd ApiCoreRefibra
3. docker-compose build
4. docker-compose up

##Navigate

1. A API disponível em: http://localhost:8000/swagger
2. Web Site disponível em: http://localhost:4200
# Licenses
[TODO]

# Others informations

### Article about Wifikier according creators
Janez Brank, Gregor Leban, Marko Grobelnik. Annotating Documents with Relevant Wikipedia Concepts. Proceedings of the Slovenian Conference on Data Mining and Data Warehouses (SiKDD 2017), Ljubljana, Slovenia, 9 October 2017. 

### Links

https://jena.apache.org/documentation/fuseki2/
http://wikifier.org/
https://www.w3.org/RDF/
https://github.com/dotnet/core
https://www.dotnetrdf.org/
