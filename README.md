# ENGHackathon2020 Project

Below you will find details pertaining to the building and running of the RevitAPI addin

## RevitAPI Addin

### Building

- Addin project targets `.NET 4.7.2`
- UtilityStandard builds against `.NETStandard 2.0`

### Dependencies

- Newtonsoft.Json [12.0.3]
- Revit.RevitAPI.x64 [2019.0.0]
- Revit.RevitAPIUI.x64 [2019.0.0]
- [NATS.Client](https://github.com/nats-io/nats.net) [0.10.1]
    - Build targeting .NET Standard 2.0 can be found in Libs directory

### Running

External NATS service must be running. This is not being shipped with the addin as there is no guarentee of administrative privilages being granted to the Revit user so an external service is safer.

### Testing

A test client is also included that can be easily compiled to issue commands to the Revit application. The default test script is seen below

```
private static async Task Test(ICommunicator comms)
{
    // Issue command to get all elements of a given type
    // E.e. <Autodesk.Revit.DB.Material>
    Message response = await comms.Request(Communicator.TO_SERVER_CHANNEL, new Message
    {
        Type = "GET_ALL",
        Data = JObject.FromObject(new
        {
            Type = "Autodesk.Revit.DB.Material"
        })
    });

    // Parse response to DTO
    List<Material> dataSet = JArray.FromObject(response.Data).
                                    Select(x => (JObject)x).
                                    Select(x => x.ToObject<Material>()).
                                    ToList();

    // Select a specific item
    Material mat = dataSet.FirstOrDefault(
        d => true // e.g. d.Id == "1389804"
    );

    // Update model
    mat.Color = new Color
    {
        Red = 0xff,
        Green = 0x0,
        Blue = 0x0
    };

    // Issue command to update a given element
    Message response2 = await comms.Request(Communicator.TO_SERVER_CHANNEL, new Message
    {
        Type = "SET",
        Data = JObject.FromObject(mat)
    });
}
```