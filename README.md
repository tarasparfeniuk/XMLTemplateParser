# XMLTemplateParser

## How to use
```csharp
// ------------------------------------------------------------------------------------ //
// Create parsing map from template
// ------------------------------------------------------------------------------------ //

var mappingConfig = new XmlTemplateMappingConfiguration("@", ":");
var xDoc = XDocument.Load("template_test.xml");
var templateMapBuilder = new TemplateMapBuilder(mappingConfig);

Dictionary<string, XAttribute> map = templateMapBuilder.CreateTemplateMap(xDoc.Root);

// ------------------------------------------------------------------------------------ //
// Parsing document using created map
// ------------------------------------------------------------------------------------ //

var xmlParser = new XmlTemplateParser(map, mappingConfig);

var entity = xmlParser.ParseDocument(XDocument.Load("doc_test.xml"));

// ------------------------------------------------------------------------------------ //
// Mapping dictionary to concrete entity
// ------------------------------------------------------------------------------------ //

var internalEntity = new Dictionary<string, object>();
internalEntity.Add("Value", "test_internal");

var internalList = new List<Dictionary<string, object>>();
internalList.Add(internalEntity);
internalList.Add(internalEntity);

var testEntity = new Dictionary<string, object>();

testEntity.Add("Name", "test");
testEntity.Add("Entity", internalEntity);
testEntity.Add("Entities", internalList);

var mapper = new DictionaryToEntityMapper();

var outEntity = mapper.Map<TestEntity>(testEntity);

// ------------------------------------------------------------------------------------ //
```

## Template example
```xml
<?xml version="1.0" encoding="UTF-8"?>
	<DocumentBody DocumentCode="@Object.DocumentCode" DocumentType="@Type">
		<SomeContent>
			<Infos>
				<ExtendedInfo Type="CS" Code="VERSION" Value="@Object.Version"/>
				<ExtendedInfo Type="CS" Code="STATUS" Value="@Object.Status"/>
			</Infos>
			
			<Session SessionCode="@Object.Sessions:.Code" StartDate="@Object.Sessions:.StartDate" EndDate="@Object.Sessions:.EndDate" Leadin="0:00" Venue="JAL" VenueName="ex@mp.le" SessionStatus="SCHEDULED" SessionType="MOR">
				<SessionName Value="@Object.Sessions:.Name" Language="EN"/>
			</Session>
			
			<Unit Code="@Object.Units:.Code" PhaseType="3" ScheduleStatus="SCHEDULED" StartDate="2018-02-11T11:00:00+09:00" EndDate="2018-02-11T13:10:00+09:00" Medal="1" Venue="JAL" Location="JAL" SessionCode="ALP01">
				<ItemName Language="@Object.Units:.Items:.Language" Value="@Object.Units:.Items:.Value"/>
				<VenueDescription VenueName="@Object.Units:.Venue.Description" LocationName="@Object.Units:.Venue.Name"/>
			</Unit>
		</SomeContent>
	</DocumentBody>
```
