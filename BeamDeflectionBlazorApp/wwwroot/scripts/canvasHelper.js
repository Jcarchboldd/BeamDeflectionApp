function drawBeam(deflections, loadSegment) {
    var canvas = document.getElementById('beamCanvas');
    var ctx = canvas.getContext('2d');

    // Clear the canvas
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Draw grid lines
    ctx.strokeStyle = '#e0e0e0';  // Light gray color for the grid
    ctx.lineWidth = 1;
    for (let i = 50; i <= 750; i += 70) {
        ctx.beginPath();
        ctx.moveTo(i, 50);
        ctx.lineTo(i, 150);
        ctx.stroke();
    }
    for (let i = 50; i <= 150; i += 10) {
        ctx.beginPath();
        ctx.moveTo(50, i);
        ctx.lineTo(750, i);
        ctx.stroke();
    }

    // Draw the beam as a straight line (original shape)
    ctx.strokeStyle = 'blue';  // Color of the original beam line
    ctx.lineWidth = 4;
    ctx.beginPath();
    ctx.moveTo(50, 100);
    ctx.lineTo(750, 100);
    ctx.stroke();

    // Draw load arrow at the specified segment
    ctx.strokeStyle = 'red';  // Color of the load arrow
    ctx.fillStyle = 'red';
    let x = 50 + loadSegment * 70; // Position along the canvas
    let loadY = 100;               // Position where load is applied

    ctx.beginPath();
    ctx.moveTo(x, loadY);  // Start at the load position
    ctx.lineTo(x, loadY + 20); // Draw downward
    ctx.stroke();
    ctx.beginPath();
    ctx.moveTo(x - 5, loadY + 15); // Left part of the arrowhead
    ctx.lineTo(x, loadY + 20);     // Tip of the arrow
    ctx.lineTo(x + 5, loadY + 15); // Right part of the arrowhead
    ctx.fill();

    // Draw the deflected shape of the beam
    ctx.strokeStyle = 'green';  // Color of the deflected line
    ctx.lineWidth = 2;
    ctx.beginPath();
    let keys = Object.keys(deflections);
    for (let i = 0; i < keys.length; i++) {
        let x = 50 + i * 70; // Position along the canvas
        let y = 100 - deflections[keys[i]]; // Adjust for millimeters directly

        if (i === 0) {
            ctx.moveTo(x, y); // Start the deflected line
        } else {
            ctx.lineTo(x, y); // Draw the deflected line
        }

        // Draw node labels
        ctx.fillStyle = 'black';
        ctx.font = '12px Arial';
        ctx.textAlign = 'center';
        ctx.fillText(`Node ${i}`, x, 90); // Label the node above the beam
    }
    ctx.stroke();
}