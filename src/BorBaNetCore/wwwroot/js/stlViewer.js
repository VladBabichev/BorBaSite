
//var currentMaterialColor = 0xcfcfcf;

var isFirstLoaded = true;
var currentMat = new JSC3D.Material();

var mat1 = new JSC3D.Material();
mat1.simulateSpecular = false;
mat1.diffuseColor = currentMaterialColor;
mat1.transparency = 0;

var mat2 = new JSC3D.Material();
mat2.simulateSpecular = false;
mat2.diffuseColor = 0x323230;
mat2.transparency = 0;

var mat3 = new JSC3D.Material();
mat3.simulateSpecular = false;
mat3.diffuseColor = 0x21160d;
mat3.transparency = 0;

var mat4 = new JSC3D.Material();
mat3.simulateSpecular = false;
mat3.diffuseColor = 0xFFFF00;
mat3.transparency = 0;

currentMat = mat1;
var scene = viewer.getScene();

var pointedObject = null;
var selectedItem = null;
var ctx = canvas.getContext('2d');

var pointerX = -1;
var pointerY = -1;
var isPointerMoving = false;

var isTipOn = false;
var tipText = '';
var tipTextMargin = 5;
var tipTextHeight = 12;
var tipTextFont = '12px Courier New'
var tipTextColor = '#000000';
var tipBackgroundColor = '#FFFF00';
var tipFrameColor = '#000000';
var tipOffsetX = 0;
var tipOffsetY = 16;
var tipOffsetStartY = 16;
var canvasHeightStart = canvas.height;
var tipTimerID = 0;

var pathExt = '.STL';
var theScene = new JSC3D.Scene;
var totalLoaded = 0;
document.body.style.cursor = 'wait';
canvas.addEventListener("mouseout", canvasMouseOut, false);

// put main script here



// end of

//resize();
var currentCanvasWidth = canvas.width;
var currentCanvasHeight = canvas.height;
var currentWindowHeight = window.innerHeight;
var currentWindowWidth = window.innerWidth;
var _mesh = null;

function test() {
    alert(viewer.initRotX);
    alert(viewer.initRotY);
    alert(viewer.initRotZ);
}

function drawTip() {
    if (isTipOn) {
        ctx.font = tipTextFont;

        var textWidth = ctx.measureText(tipText).width;
        var rectLeft = pointerX + tipOffsetX;
        var rectRight = rectLeft + textWidth + tipTextMargin * 2;
        var rectTop = pointerY + tipOffsetY;

        var rectBottom = rectTop + tipTextHeight + tipTextMargin * 2;

        // generate a rounded rectangle as the tip box
        ctx.beginPath();
        ctx.moveTo(rectLeft, rectTop + 2);
        ctx.quadraticCurveTo(rectLeft, rectTop, rectLeft + 2, rectTop);
        ctx.lineTo(rectRight - 2, rectTop);
        ctx.quadraticCurveTo(rectRight, rectTop, rectRight, rectTop + 2);
        ctx.lineTo(rectRight, rectBottom - 2);
        ctx.quadraticCurveTo(rectRight, rectBottom, rectRight - 2, rectBottom);
        ctx.lineTo(rectLeft + 2, rectBottom);
        ctx.quadraticCurveTo(rectLeft, rectBottom, rectLeft, rectBottom - 2);
        ctx.closePath();

        // draw frame of tip box
        ctx.lineWidth = 1;
        ctx.strokeStyle = tipFrameColor;
        ctx.stroke();
        // draw tip box
        //ctx.fillStyle = tipBackgroundColor;
        //ctx.fill();
        // draw tip text
        //ctx.fillStyle = tipTextColor;
        //ctx.fillText(tipText , rectLeft + tipTextMargin, rectTop + tipTextMargin + tipTextHeight - 2);
        //ctx.fillText(pointerX, rectLeft + tipTextMargin, rectTop + tipTextMargin + tipTextHeight - 2);

        //lblCurrentSection.SetValue(tipText);

        $('.lblCurrentSection').innerText = tipText;
        var span = document.getElementById("lblCurrentSection");
        span.innerText = tipText;
    }
    else {
        // call viewer to repaint to erase the tip stuff
        viewer.update(true/*repaint only*/);
    }
}


// events
function onMousemove(x, y, button, depth, mesh) {
    if (isFirstLoaded === true) {
        isFirstLoaded = false;
        this.zoomFactor = zoomFactor;
    }
    if (mesh !== null) {
        _mesh = mesh;
        if (isTipOn && (x !== pointerX || y !== pointerY)) {
            isTipOn = false;
            //drawTip();
        }

        //lblCurrentSection.SetValue(mesh.name);
        $('.lblCurrentSection').Text = mesh.name;
        var span = document.getElementById("lblCurrentSection");
        span.innerText = mesh.name;
        //alert(mesh);
        pointerX = x;
        pointerY = y;
        isPointerMoving = true;

        if (pointedObject !== null && pointedObject !== selectedItem) {
            pointedObject.setMaterial(currentMat);
        }

        //currentMat = mesh.material;
        pointedObject = mesh;

        // set the new material to the selected mesh
        if (mat3 !== null) {
            pointedObject.setMaterial(mat3);
        }
        // tell viewer to render a new frame
        viewer.update();
    }
};

function onTimer() {
    //	if (pointedObject!==null && mat3!==null) {
    //		pointedObject.setMaterial(mat3);
    //	}
    //	if (isPointerMoving) {
    //		isPointerMoving = false;
    //	}
    //	else if (!isTipOn && pointedObject != null) {
    //		// the pointer stays at an object. show tip on it.
    //		isTipOn = true;	
    //		tipText = pointedObject.name;
    //		drawTip();
    //	}

    if (isFirstLoaded === true) {
        //isFirstLoaded = false;
        this.zoomFactor = zoomFactor;
        viewer.update();
        //alert(this.zoomFactor);
    }
}

function resize() {
    this.zoomFactor = zoomFactor;
    //splitter.AdjustControl();

    var windowHeight = window.innerHeight;
    var windowWidth = window.innerWidth;
    var deltaHeight = windowHeight - currentWindowHeight;
    var deltaWidth = window.innerWidth - currentWindowWidth;

    var canvasRatio = canvas.height / canvas.width;
    var windowRatio = window.innerHeight / window.innerWidth;
    var width;
    var height;

    //alert(window.innerHeight);
    //alert(window.innerWidth);

    //	if (windowRatio < canvasRatio) {
    //		height = ctxCanvas.innerHeight;
    //		width = height / canvasRatio;
    //	} else {
    //		width = ctxCanvas.innerWidth;
    //		height = width * canvasRatio;
    //	}

    //currentWindowWidth = windowWidth;
    //currentCanvasWidth = currentCanvasWidth + deltaWidth/2;
    //ctx = canvas.getContext('2d');
    //canvas.width = currentCanvasWidth;
    //ctx.width = currentCanvasWidth;
    //ctx.canvas.width = 600;
    //ctx.canvas.height = height ;

    //	var ratio1 = width/oldWidth ;
    //	var ratio2 = height / oldHeight;

    //var mm = viewer.rotMatrix;
    //mm.identity();

    //ctx.scale(2, 2,2);
    //ctx.translate(width / 2, height / 2);
    //viewer = new JSC3D.Viewer(canvas);

    //viewer.update();
    //viewer.update(true);
    //viewer.rotMatrix.scale(2, 2, 2);
    //viewer.init();
    //ctx.canvas.style.width = currentWindowWidth + 'px';
    //canvas.style.height = height + 'px';

    //loadModel();

    //var div = document.getElementById('divId');

    //div.width = currentCanvasWidth;
    //viewer.update();
};

//window.addEventListener('resize', resize, false);
//window.addEventListener('mouseup', resize, false);


function initRotMatrix() {
    m00 = viewer.rotMatrix.m00;
    m01 = viewer.rotMatrix.m01;
    m02 = viewer.rotMatrix.m02;
    m03 = viewer.rotMatrix.m03;

    m10 = viewer.rotMatrix.m10;
    m11 = viewer.rotMatrix.m11;
    m12 = viewer.rotMatrix.m12;
    m13 = viewer.rotMatrix.m13;

    m20 = viewer.rotMatrix.m20;
    m21 = viewer.rotMatrix.m21;
    m22 = viewer.rotMatrix.m22;
    m23 = viewer.rotMatrix.m23;
}


function onMousedown(x, y, button, depth, mesh) {
    //button == 0/*left button down*/ &&  
    if (isFirstLoaded === true) {
        //isFirstLoaded = false;
        this.zoomFactor = zoomFactor;
    }
    if (mesh !== null) {
        if (selectedItem !== null) {
            selectedItem.setMaterial(currentMat);
        }

        if (pointedObject !== null) {
            pointedObject.setMaterial(currentMat);
        }
        selectedItem = mesh;
        if (mat3 !== null) {
            selectedItem.setMaterial(mat3);
        }
        viewer.update();
        // lblSelectedSection.SetValue(mesh.name);
        $('.lblSelectedSection').innerText = mesh.name;
        var span = document.getElementById("lblSelectedSection");
        span.innerText = mesh.name;
        grdSectionItems.PerformCallback('Model3D;' + mesh.name);
    }
};

function initViewer() {
    viewer.onmousedown = onMousedown;
    viewer.onmousemove = onMousemove;
    viewer.replaceScene(theScene);
    viewer.zoomFactor = zoomFactor;

    this.frameWidth = this.canvas.width;
    this.frameHeight = this.canvas.height;


    viewer.update();
    initRotMatrix();
    //setInterval(onTimer, 200);
    document.body.style.cursor = '';
    //alert();
    click(0, 0);

    //alert(ctx.canvas.width + ' ' + window.innerWidth);
}

function click(x, y) {
    //alert();
    var ev = document.createEvent("MouseEvent");
    var el = document.elementFromPoint(x, y);
    ev.initMouseEvent(
        "click",
        true /* bubble */, true /* cancelable */,
        window, null,
        x, y, 0, 0, /* coordinates */
        false, false, false, false, /* modifier keys */
        0 /*left*/, null
    );

    if (typeof (ei) !== 'undefined' && ei !== null) {
        el.dispatchEvent(ev);
    }
}

function canvasMouseOut(event) {
    var x = event.pagex;
    if (pointedObject !== null && pointedObject !== selectedItem) {
        pointedObject.setMaterial(currentMat);
        viewer.update();
    }
}

function setMeshColor(name) {
    if (name !== null) {
        var scene = viewer.getScene();
        var meshes = scene.getChildren();
        var result = '';
        for (var i = 0; i < meshes.length; i++) {
            var j = meshes[i].name.indexOf('-');
            result = meshes[i].name;
            if (j > -1) {
                result = meshes[i].name.substring(j + 1, meshes.length - 1);
            }

            if (result.trim() === name.trim()) {
                if (selectedItem !== null) {
                    selectedItem.setMaterial(currentMat);
                }

                if (pointedObject !== null) {
                    pointedObject.setMaterial(currentMat);
                }

                meshes[i].setMaterial(mat3);
                selectedItem = meshes[i];
                viewer.update();

                //lblSelectedSection.SetValue(meshes[i].name);
                $('.lblSelectedSection').innerText = meshes[i].name;
                var span = document.getElementById("lblSelectedSection");
                span.innerText = meshes[i].name;
                //lblCurrentSection.SetValue(meshes[i].name);
                $('.lblCurrentSection').innerText = meshes[i].name;
                var span1 = document.getElementById("lblCurrentSection");
                span1.innerText = meshes[i].name;
            }
        }
    }
}