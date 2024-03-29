function displayTriangle(x) {
    let triangleDisplay = document.getElementById("triangleDisplay");
    triangleDisplay.innerHTML = "";
    let triangle = document.createElement("div");

    switch (x) {
        case 1:
            triangle.classList.add("right-angle");
            let image0 = document.createElement("img");
            image0.src = "rightangled.png";
            image0.classList.add("triangle-image");
            triangle.appendChild(image0);
            break;
        case 2:
            triangle.classList.add("acute");
            let image1 = document.createElement("img");
            image1.src = "acute.png";
            image1.classList.add("triangle-image");
            triangle.appendChild(image1);
            break;
        case 3:
            triangle.classList.add("obtuse");
            let image2 = document.createElement("img");
            image2.src = "obtuse.png";
            image2.classList.add("triangle-image");
            triangle.appendChild(image2);
            break;
    }

    triangleDisplay.appendChild(triangle);
}

function changeTriangleType(type) {
    // Call the displayTriangle function with the selected type
    displayTriangle(type);
}

async function sendMessage() {
    console.log("Button pressed");

    // Get the values from the textboxes
    let side0 = parseFloat(document.getElementById("side0").value);
    let side1 = parseFloat(document.getElementById("side1").value);
    let side2 = parseFloat(document.getElementById("side2").value);
    let angle0 = parseFloat(document.getElementById("angle0").value);
    let angle1 = parseFloat(document.getElementById("angle1").value);
    let angle2 = parseFloat(document.getElementById("angle2").value);

    if (isNaN(side0)) { side0 = null }
    if (isNaN(side1)) { side1 = null }
    if (isNaN(side2)) { side2 = null }
    if (isNaN(angle0)) { angle0 = null }
    if (isNaN(angle1)) { angle1 = null }
    if (isNaN(angle2)) { angle2 = null }


    // Create a triangle object
    let triangle = {
        side0: side0,
        side1: side1,
        side2: side2,
        angle0: angle0,
        angle1: angle1,
        angle2: angle2
    };

    // Convert the triangle object to JSON
    let triangleJson = JSON.stringify(triangle);

    // Send the triangle data to the server
    let response = await fetch("/message", {
        method: "POST",
        body: triangleJson,
    });

    console.log("Sent triangle");
    console.log(triangle);
    /**
         * @type {{
         *  Side0: number | null,
         *  Side1: number | null,
         *  Side2: number | null,
         *  angle0: number | null,
         *  angle1: number | null,
         *  angle2: number | null
         * }}
         */
    // Parse the response JSON
    let triangleNew = await response.json();
    console.log("Received triangle");
    console.log(triangleNew);
    document.getElementById("side0").value = triangleNew.side0 ;
    document.getElementById("side1").value = triangleNew.side1 ;
    document.getElementById("side2").value = triangleNew.side2 ;
    document.getElementById("angle0").value = triangleNew.angle0 ;
    document.getElementById("angle1").value = triangleNew.angle1 ;
    document.getElementById("angle2").value = triangleNew.angle2 ;
}
