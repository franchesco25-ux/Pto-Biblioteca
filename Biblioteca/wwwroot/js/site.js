//import Swal from 'sweetalert2';


$(document).ready(function () {
    // 1. Escuchar el evento 'click' en cualquier elemento con la clase 'ver-informacion-btn'
    $('.btn-see-recurso').on('click', function (e) {

        e.preventDefault(); // Previene la acción por defecto del enlace (si es un <a>)

        // 2. Obtener el ID del producto desde el atributo 'data-id' del botón
        var productoId = $(this).data('id');

        // 3. Definir la URL a la que haremos la petición Ajax


        // Opcional: Mostrar un indicador de carga mientras se obtienen los datos
        $('#infoModalContent').html('<div class="text-center p-5"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Cargando...</span></div></div>');
        $('#infoModal').modal('show'); // Mostramos el modal con el spinner

        // 4. Realizar la petición Ajax con jQuery
        $.ajax({
            url: url,
            type: 'GET',
            data: { id: productoId }, // Enviamos el ID como parámetro en la URL
            success: function (response) {
                // 5. Si la petición es exitosa, reemplazamos el contenido del modal
                // con la respuesta (que es el HTML de la Vista Parcial)
                $('#infoModalContent').html(response);

                // 6. Volvemos a mostrar el modal, ahora con los datos correctos.
                // Esto es útil si la carga fue muy rápida, asegura que el modal se muestre.
                $('#infoModal').modal('show');
            },
            error: function (xhr, status, error) {
                // 7. Si hay un error, lo mostramos en la consola y al usuario.
                console.error("Error en la llamada Ajax: ", error);
                var errorHtml = '<div class="modal-header"><h5 class="modal-title">Error</h5></div>' +
                    '<div class="modal-body"><p>Ocurrió un error al cargar la información. Por favor, intente de nuevo.</p></div>' +
                    '<div class="modal-footer"><button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button></div>';
                $('#infoModalContent').html(errorHtml);
                $('#infoModal').modal('show');
            }
        });
    });

    $('#logoutform').on('submit', function (e) {
        e.preventDefault(); // Previene la acción por defecto del enlace (si es un <a>)
        Swal.fire({
            title: '¿Estás seguro que deseas cerrar sesión?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, salir',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            // Si el usuario confirma
            if (result.isConfirmed) {
                // Envía el formulario de forma manual
                e.target.submit();
            }
        });
    });

    $('.btndeleterecurso').on('click', function (e) {
        e.preventDefault(); // Previene la acción por defecto del enlace (si es un <a>)
        Swal.fire({
            title: '¿Estás seguro que deseas eliminar?',
            icon: 'danger',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, eliminar recurso',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            // Si el usuario hace clic en "Sí, continuar"
            if (result.isConfirmed) {
                // Obtiene los datos del enlace
                const urlBase = $(this).data('url');
                const id = $(this).data('id');

                // Construye la URL final
                const urlFinal = `${urlBase}/${id}`;

                // Navega a la nueva URL
                window.location.href = urlFinal;
            }
        });
    });
});

