window.descargarArchivo = function (nombreArchivo, base64) {

    try {

        const byteCharacters = atob(base64);

        const byteNumbers = new Array(byteCharacters.length);

        for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
        }

        const byteArray = new Uint8Array(byteNumbers);

        const blob = new Blob(
            [byteArray],
            {
                type: "application/pdf"
            }
        );

        const url = URL.createObjectURL(blob);

        const link = document.createElement("a");

        link.href = url;
        link.download = nombreArchivo;

        document.body.appendChild(link);

        link.click();

        document.body.removeChild(link);

        URL.revokeObjectURL(url);

    }
    catch (error) {

        console.error(
            "Error al descargar el PDF:",
            error
        );

    }
};