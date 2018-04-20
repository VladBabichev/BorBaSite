
function drawText(cnv, text) {
    var canvas = document.getElementById(cnv);
    var context = canvas.getContext('2d');
    context.font = "30px serif bolt";
    context.fillStyle = "white"; // blue
    context.textAlign = "center";

    //context.strokeStyle = "yellow";


    var unit = canvas.height / 4;
    var x = canvas.width / 2;
    var y = canvas.height / 2;
    
    // context.strokeRect(x, y, textWidth, height,fontSize * 1.286);
    // draw relative to translate point
    context.fillText(text, x, y);

    //y = canvas.height - 3 * unit;
    //context.fillText('MVC', x, y);

    //var ctx2 = document.getCSSCanvasContext("2d", "mybackground", 300, 300);
    //ctx2.globalAlpha = 0.5;
    //ctx2.drawImage(canvas, 0, 0);
}

//setInterval(rotate, 100);

function drawRec(cnv, text) {
    var canvas = document.getElementById(cnv);
    var context = canvas.getContext('2d');
    var x = canvas.width / 2;
    var y = canvas.height / 2;
    var width = context.measureText(text).width;
    var fontSize = 24;
    var height = fontSize * 1.286;
    context.beginPath();
    //context.rect(5, y - fontSize, canvas.width - 10, height + 5);

    roundRect(context, 5, y - fontSize, canvas.width - 10, height + 5, 15);
    context.fillStyle = 'Green';
    context.fill();
    context.lineWidth = 1;
    context.strokeStyle = 'yellow';
    context.stroke();
}

function roundRect(context, x, y, w, h, radius) {
    var r = x + w;
    var b = y + h;
    context.beginPath();
    context.strokeStyle = "white";
    context.lineWidth = "2";
    context.moveTo(x + radius, y);
    context.lineTo(r - radius, y);
    context.quadraticCurveTo(r, y, r, y + radius);
    context.lineTo(r, y + h - radius);
    context.quadraticCurveTo(r, b, r - radius, b);
    context.lineTo(x + radius, b);
    context.quadraticCurveTo(x, b, x, b - radius);
    context.lineTo(x, y + radius);
    context.quadraticCurveTo(x, y, x + radius, y);
    context.stroke();
}

function loadcnv() {
    var canvas = document.getElementById('cnv1');
    var context = canvas.getContext('2d');
    //var x = canvas.width / 2;
    //var y = canvas.height / 2;
    var x = 0;
    var y = 0;
    var radius = 75;
    var offset = 50;

    /*
     * save() allows us to save the canvas context before
     * defining the clipping region so that we can return
     * to the default state later on
     */
    context.save();
    context.beginPath();
    context.arc(x, y, radius, 0, 2 * Math.PI, false);
    context.clip();

    // draw blue circle inside clipping region
    context.beginPath();
    context.arc(x - offset, y - offset, radius, 0, 2 * Math.PI, false);
    context.fillStyle = 'blue';
    context.fill();

    //// draw yellow circle inside clipping region
    //context.beginPath();
    //context.arc(x + offset, y, radius, 0, 2 * Math.PI, false);
    //context.fillStyle = 'yellow';
    //context.fill();

    //// draw red circle inside clipping region
    //context.beginPath();
    //context.arc(x, y + offset, radius, 0, 2 * Math.PI, false);
    //context.fillStyle = 'red';
    //context.fill();

    /*
     * restore() restores the canvas context to its original state
     * before we defined the clipping region
     */
    context.restore();
    context.beginPath();
    context.arc(x, y, radius, 0, 2 * Math.PI, false);
    context.lineWidth = 10;
    context.strokeStyle = 'blue';
    context.stroke();
}

function drowImg(cnv, imgUrl) {
    var canvas = document.getElementById(cnv);
    var context = canvas.getContext('2d');
    var imageObj = new Image();
    imageObj.src = imgUrl;  

    imageObj.onload = function () {
        var imageAspectRatio = 0;
        var canvasAspectRatio = canvas.width / canvas.height;
        if (imageObj.height > 0) {
           imageAspectRatio = imageObj.width / imageObj.height;            
        }
       
        var renderableHeight, renderableWidth, xStart, yStart;
        // If image's aspect ratio is less than canvas's we fit on height
        // and place the image centrally along width
        if (imageAspectRatio < canvasAspectRatio & imageAspectRatio > 0) {
            renderableHeight = canvas.height;
            renderableWidth = imageObj.width * (renderableHeight / imageObj.height);
            xStart = (canvas.width - renderableWidth) / 2;
            yStart = 0;
           // debugger;
        }
        // If image's aspect ratio is greater than canvas's we fit on width
        // and place the image centrally along height
        else if (imageAspectRatio > canvasAspectRatio & imageAspectRatio > 0) {
            renderableWidth = canvas.width
            renderableHeight = imageObj.height * (renderableWidth / imageObj.width);
            xStart = 0;
            yStart = (canvas.height - renderableHeight) / 2;
           // debugger;
        }
      
        // Happy path - keep aspect ratio
        else {
           // debugger;
            xStart = 0;
            yStart = 0;
            renderableHeight = canvas.height;
            renderableWidth = canvas.width;
            //if ( canvas.height > canvas.width) {
            //    renderableHeight = canvas.width;
            //    renderableWidth = canvas.width;
            //    xStart = (canvas.width - imageObj.width) / 2;
            //}
            //else if ( canvas.height < canvas.width) {
            //    renderableHeight = canvas.height;
            //    renderableWidth = canvas.height;
            //    xStart = (canvas.height - imageObj.width) / 2;
            //}
          
        }
        context.drawImage(imageObj, xStart+5, yStart+5, renderableWidth-5, renderableHeight-10);
        //context.drawImage(imageObj, 0, 0, canvas.width, canvas.height);
    };
    //imageObj.src = imgUrl;
    //imageObj.width='200';
    imageObj.radius = 10;
    //context.lineWidth = 1;
    //context.strokeStyle = "black";
    //context.rect(0, 0, canvas.width, canvas.height);
    //context.fillRect(0, 0, canvas.width, canvas.height);
    context.fillStyle = 'white';
    //context.shadowColor = "darkgray";
    //context.shadowBlur = 25;
    //context.shadowOffsetX = 5;
    //context.shadowOffsetY = 5;
    context.fill();

    //context.lineWidth = 4;
    //context.strokeStyle = "white";
    //context.strokeRect(0, 0, canvas.width, canvas.height);//for white background
    //context.scale(2, 2);
    //context.fillStyle = "black";
    //context.font = "50px Arial";

    //context.fillText('BorBa', 0, 50);
    //context.globalCompositeOperation = "destination-over";
    //context.fillStyle = "white";
    //context.fillRect(0, 0, canvas.width, canvas.height);//for white background
    //context.globalCompositeOperation = "source-over";
    //context.lineWidth = 5;
    //context.strokeStyle = "white";
    //context.strokeRect(0, 0, canvas.width, canvas.height);//for white background

    //
  
}

function crop() {
    imageObj.onload = function () {
        // draw cropped image
        var sourceX = 150;
        var sourceY = 0;
        var sourceWidth = 150;
        var sourceHeight = 150;
        var destWidth = sourceWidth;
        var destHeight = sourceHeight;
        var destX = canvas.width / 2 - destWidth / 2;
        var destY = canvas.height / 2 - destHeight / 2;

        context.drawImage(imageObj, sourceX, sourceY, sourceWidth, sourceHeight, destX, destY, destWidth, destHeight);
    };
    imageObj.src = 'http://www.html5canvastutorials.com/demos/assets/darth-vader.jpg';
    imageObj.radius = 10;

}

function drawText1(cnv, text) {
    var canvas = document.getElementById(cnv);
    var context = canvas.getContext('2d');
    context.font = "30px serif bolt";
    context.fillStyle = "white"; // blue
    context.textAlign = "center";

    //context.strokeStyle = "yellow";


    var unit = canvas.height / 4;
    var x = canvas.width / 2;
    var y = canvas.height / 2;

    // get every characters positions
    var textWidth = 0;
    var chars = [];
    for (var i = 0; i < text.length; i++) {
        chars.push(i);
    }
    var fontSize = 30;
    //iterate through the characters list
    for (var i = 0; i < chars.length; i++) {
        // get the x position of this character
        var xPos = x + context.measureText(text.substring(0, chars[i])).width;
        // get the width of this character
        var width = context.measureText(text.substring(chars[i], chars[i] + 1)).width;
        textWidth = textWidth + width;
        // get its height through the 1.286 approximation
        var height = fontSize * 1.286;
        // get the y position
        var yPos = y - height / 1.5
        // draw the rect
        //context.strokeRect(xPos, yPos, width, height);
    }

    // context.strokeRect(x, y, textWidth, height,fontSize * 1.286);
    // draw relative to translate point
    context.fillText(text, x, y);


    //y = canvas.height - 3 * unit;
    //context.fillText('MVC', x, y);

    //var ctx2 = document.getCSSCanvasContext("2d", "mybackground", 300, 300);
    //ctx2.globalAlpha = 0.5;
    //ctx2.drawImage(canvas, 0, 0);
}

function rotate(cnv,text,dg) {
    var canvas = document.getElementById(cnv);
    var context = canvas.getContext('2d');
    //debugger;
   //context.save();



    context.font = "16px serif";
    context.fillStyle = "#0000ff"; // blue
    context.textAlign = "left";

    var x = 0;
    var y = canvas.height / 2;
    // draw relative to translate point
    context.fillText(text, x, y);


    // hold top-right hand corner when rotating
    //context.translate(canvas.width / 2, canvas.height / 2);

    //context.rotate(dg * Math.PI / 180);
   //context.restore();
}

function drowTransparentImg(url,cnv,w,h) {   
    var L = w * h;
    var Img = new Image();
    canvas = document.getElementById(cnv);
    if (canvas.getContext) {
        Rxt = canvas.getContext("2d");
        Img.onload = function () {
            Rxt.drawImage(Img, 0, 0);
            GetColor(Rxt);
            PutColor(Rxt, Img);
        }
        Img.src = url;
    }
}

function GetColor(Rxt,w,h) {
    Img = Rxt.getImageData(0, 0, w, h);
    for (var i = 0; i < L * 4; i += 4) {
        Img.data[i + 3] = 10;
    }
}
function PutColor(Rxt, Img) {
    Rxt.putImageData(Img, 0, 0);
}

