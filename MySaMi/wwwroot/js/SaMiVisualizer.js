$(document).ready(function () {
    const chartOptions = {
        scales: {
            yAxes: [{
                ticks: {
                    beginAtZero: false
                }
            }]
        }
    };

    const context = $("#chart");

    let validKey;

    let sensors;

    $("#get-sensors-saved").click(function (event) {
        event.preventDefault();

        if ($("#key-select").val() == "") {
            return;
        }

        $("#get-sensors-saved").prop("disabled", true);
        $("#get-sensors-saved").html("Working..");

        $.ajax({
            method: "GET",
            url: sensorsUrl,
            data: {
                key: $("#key-select").val()
            },
            success: function (data) {
                $("#sensor-select").empty();

                sensors = data;

                sensors.forEach(function (sensor) {
                    $("#sensor-select").append(`<option value=${sensor.Tag}>${sensor.Name}</option>`);
                });

                validKey = $("#key-select").val();

                $("#sensor-row").removeClass("invisible");

                $("#date-row").removeClass("invisible");
            },
            error: function (xhr, status, error) {
                // ToDo: Less annoying invalid key message

                alert("Invalid key.");
            },
            complete: function () {
                $("#get-sensors-saved").html("Get sensors");
                $("#get-sensors-saved").prop("disabled", false);
            }
        });
    });

    $("#get-sensors").click(function (event) {
        event.preventDefault();

        if ($("#sami-key").val() == "") {
            return;
        }

        $("#get-sensors").prop("disabled", true);
        $("#get-sensors").html("Working..");
        
        $.ajax({
            method: "GET",
            url: sensorsUrl,
            data: {
                key: $("#sami-key").val()
            },
            success: function (data) {
                $("#sensor-select").empty();

                sensors = data;

                sensors.forEach(function (sensor) {
                    $("#sensor-select").append(`<option value=${sensor.Tag}>${sensor.Name}</option>`);
                });

                validKey = $("#sami-key").val();

                $("#sensor-row").removeClass("invisible");

                $("#date-row").removeClass("invisible");
            },
            error: function (xhr, status, error) {
                // ToDo: Less annoying invalid key message

                alert("Invalid key.");
            },
            complete: function () {
                $("#get-sensors").html("Get sensors");
                $("#get-sensors").prop("disabled", false);
            }
        });
    });

    $("#get-measurements").click(function (event) {
        event.preventDefault();

        if ($("#date").val() === "" || $("#sensor-select").val() === "") {
            return;
        }

        let sensor = sensors.filter(function (element) {
            return element.Tag == $("#sensor-select").val();
        });

        if (sensor === undefined) {
            return;
        }

        $("#get-measurements").prop("disabled", true);
        $("#get-measurements").html("Working..");

        $.ajax({
            method: "GET",
            url: measurementsUrl,
            data: {
                Key: validKey,
                Tag: sensor[0].Tag,
                Date: $("#date").val(),
                Unit: sensor[0].Unit,
                Name: sensor[0].Name
            },
            success: function (data) {
                data.datasets[0].backgroundColor = "rgba(252,147,65,0.5)";

                let chart = new Chart(context, {
                    type: "bar",
                    data: data,
                    options: chartOptions
                });

                chart.update();
            },
            error: function () {
                $("#get-measurements").html("Get measurements");
                $("#get-measurements").prop("disabled", false);
            },
            complete: function () {
                $("#get-measurements").html("Get measurements");
                $("#get-measurements").prop("disabled", false);
            }
        })
    });
});