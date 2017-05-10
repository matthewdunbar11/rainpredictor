var express = require('express')
var app = express()
var Client = require('node-rest-client').Client;
var Horseman = require('node-horseman');


app.set('port', (process.env.PORT || 5000))
app.use(express.static(__dirname + '/public'))

app.get('/request', function (request, response) {
    var zip = request.query.zip;



    var url = "https://www.ncdc.noaa.gov/cdo-web/api/v2/stations?locationid=ZIP:" + zip;

    var client = new Client();
    var args = {
        headers: { "token": "ZAPBmWXmYxQKkspfDPKlBODFcYCxBDVs" }
    };

    client.get(url, args, function (data, response) {
        var correctEntry;
        for (var index = 0; index < data.results.length; index++)
        {
            var currentEntry = data.results[index];
            if (currentEntry.id.includes("WBAN"))
            {
                correctEntry = currentEntry;
            }
        }

        var targetUri = "https://www.ncdc.noaa.gov/cdo-web/datasets/LCD/stations/" + correctEntry.id + "/detail";
        var horseman = new Horseman();

        var h = horseman
            .open(targetUri);

        h.click("a:contains('ADD TO CART')");
        h.open("https://www.ncdc.noaa.gov/cdo-web/cart");
        h.waitForNextPage();
        h.click("input#LCD_CUSTOM_CSV");
        h.type("input.noaa-daterange-input", "2010-01-01 to " + (new Date()).toISOString().substring(0, 10));
        h.click("button:contains('Continue')");
        h.waitForNextPage();
        h.type("input#email", "matthewdunbar11@gmail.com");
        h.type("input#emailConfirmation", "matthewdunbar11@gmail.com");
        h.click("input#buttonSubmit");;
    });



    console.log('Requesting for ' + zip);

    response.write('Response received for ' + zip);

    response.end();
})

app.listen(app.get('port'), function () {
    console.log("Node app is running at localhost:" + app.get('port'))
})