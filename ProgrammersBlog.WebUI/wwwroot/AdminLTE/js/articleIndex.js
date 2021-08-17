
    $(document).ready(function () {
            const dataTable = $('#articlesTable').DataTable({
        dom:
    "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
    "<'row'<'col-sm-12'tr>>" +
    "<'row'<'col-sm-5'i><'col-sm-7'p>>",
    buttons: [
    {
        text: 'Ekle',
    attr: {
        id:"btnAdd",
                            },
    className: 'btn btn-success',
            action: function (e, dt, node, config) {
                let url = window.location.href; //şuan ki url miz
                url = url.replace("/Index", ""); //eğer sayfaya Index üzerinden ulasş Index kısmı silinip yönlend.
                window.open(`${url}/Add`, "_self"); //aynı sayfada acmak icin self degeri veriyoruz.

                
    }
                        },
    {
        text: 'Yenile',
    className: 'btn btn-warning',
    action: function (e, dt, node, config) {
        $.ajax({ //butonumuz içeris. calıs. olan jquery metodunu yazıy.
            type: 'GET', //yap. get post put vb gibi islemi seciyoruz.
            url: '@Url.Action("GetAllUsers","User")', //verileri nered. alıcag. url yi ayarl.
            contentType: "application/json", //gelic. olan veri tipini belrit. xml de olab.
            beforeSend: function () { //get isle. yap. hemen önce calıs. olan fonk. tanımla.
                $('#usersTable').hide();
                $('.spinner-border').show();
            },
            success: function (data) { //get islemi bas. old. sonra ki calıs. kısım.
                const userListDto = jQuery.parseJSON(data); //Cont. icers. yazm. old. metotdan gelen veriyi ald. sonra parse edi.
                dataTable.clear();
                console.log("gelen data userlist")
                console.log(userListDto);
                if (userListDto.ResultStatus === 0) {//veriler bas. sek. gelmis demektir.

                    $.each(userListDto.Users.$values, function (index, user) { //gelen verileri Json içeris. seciyoruz ve foreach ile dön.
                        const newTableRow = dataTable.row.add([
                            user.Id,
                            user.UserName,
                            user.Email,
                            user.PhoneNumber,
                            ` <img src="/img/${user.Picture}" alt="${user.UserName}" style="max-height:50px; max-width:50px;" />`,
                            `
                                    <button class="btn btn-primary btn-sm  btn-update" data-id="${user.Id}"><span class="fas fa-edit"></span></button>
                                    <button class="btn btn-danger btn-sm  btn-delete" data-id="${user.Id}"><span class="fas fa-minus-circle"></span></button>
                                                    `
                        ]).node();
                        const jqueryTableRow = $(newTableRow);
                        jqueryTableRow.attr('name', `${user.Id}`);
                    });
                    dataTable.draw();
                    $('.spinner-border').hide(); //yükleniyor spinner ı kapatıyoruz.
                    $('#usersTable').fadeIn(1800); // tablomuz efektli bir sek. ekrana gel.
                }
                else {
                    toastr.error(`${userListDto.Message}`, 'Islem basarısız!');
                    alert(`${userListDto.Message}`);
                }
            },
            error: function (err) { //get isle. hatalı old. sonra calıs. kısım
                console.log(err);
                $('.spinner-border').hide(); //yükleniyor spinner ı kapatıyoruz.
                $('#usersTable').fadeIn(1800); // tablomuz efektli bir sek. ekrana gel.
                toastr.error(`${err}`, 'Islem basarısız!');
                Swal.fire({
                    icon: 'error',
                    title: 'Bir hata olustu.',
                    text: `${err}`,
                    footer: '<a href="">Why do I have this issue?</a>'
                });
            }
        });
                            }
                        }
    ],
    language: {
        "emptyTable": "Tabloda herhangi bir veri mevcut değil",
    "info": "_TOTAL_ kayıttan _START_ - _END_ arasındaki kayıtlar gösteriliyor",
    "infoEmpty": "Kayıt yok",
    "infoFiltered": "(_MAX_ kayıt içerisinden bulunan)",
    "infoThousands": ".",
    "lengthMenu": "Sayfada _MENU_ kayıt göster",
    "loadingRecords": "Yükleniyor...",
    "processing": "İşleniyor...",
    "search": "Ara:",
    "zeroRecords": "Eşleşen kayıt bulunamadı",
    "paginate": {
        "first": "İlk",
    "last": "Son",
    "next": "Sonraki",
    "previous": "Önceki"
                        },
    "aria": {
        "sortAscending": ": artan sütun sıralamasını aktifleştir",
    "sortDescending": ": azalan sütun sıralamasını aktifleştir"
                        },
    "select": {
        "rows": {
        "_": "%d kayıt seçildi",
    "1": "1 kayıt seçildi",
    "0": "-"
                            },
    "0": "-",
    "1": "%d satır seçildi",
    "2": "-",
    "_": "%d satır seçildi",
    "cells": {
        "1": "1 hücre seçildi",
    "_": "%d hücre seçildi"
                            },
    "columns": {
        "1": "1 sütun seçildi",
    "_": "%d sütun seçildi"
                            }
                        },
    "autoFill": {
        "cancel": "İptal",
    "fillHorizontal": "Hücreleri yatay olarak doldur",
    "fillVertical": "Hücreleri dikey olarak doldur",
    "info": "-",
    "fill": "Bütün hücreleri <i>%d<\/i> ile doldur"
                        },
        "buttons": {
            "collection": "Koleksiyon <span class=\"ui-button-icon-primary ui-icon ui-icon-triangle-1-s\"><\/span>",
        "colvis": "Sütun görünürlüğü",
        "colvisRestore": "Görünürlüğü eski haline getir",
        "copySuccess": {
            "1": "1 satır panoya kopyalandı",
        "_": "%ds satır panoya kopyalandı"
                            },
        "copyTitle": "Panoya kopyala",
        "csv": "CSV",
        "excel": "Excel",
        "pageLength": {
            "-1": "Bütün satırları göster",
        "1": "-",
        "_": "%d satır göster"
                            },
        "pdf": "PDF",
        "print": "Yazdır",
        "copy": "Kopyala",
        "copyKeys": "Tablodaki veriyi kopyalamak için CTRL veya u2318 + C tuşlarına basınız. İptal etmek için bu mesaja tıklayın veya escape tuşuna basın."
                        },
        "infoPostFix": "-",
        "searchBuilder": {
            "add": "Koşul Ekle",
        "button": {
            "0": "Arama Oluşturucu",
        "_": "Arama Oluşturucu (%dUrl"
                            },
        "condition": "Koşul",
        "conditions": {
            "date": {
            "after": "Sonra",
        "before": "Önce",
        "between": "Arasında",
        "empty": "Boş",
        "equals": "Eşittir",
        "not": "Değildir",
        "notBetween": "Dışında",
        "notEmpty": "Dolu"
                                },
        "number": {
            "between": "Arasında",
        "empty": "Boş",
        "equals": "Eşittir",
        "gt": "Büyüktür",
        "gte": "Büyük eşittir",
        "lt": "Küçüktür",
        "lte": "Küçük eşittir",
        "not": "Değildir",
        "notBetween": "Dışında",
        "notEmpty": "Dolu"
                                },
        "string": {
            "contains": "İçerir",
        "empty": "Boş",
        "endsWith": "İle biter",
        "equals": "Eşittir",
        "not": "Değildir",
        "notEmpty": "Dolu",
        "startsWith": "İle başlar"
                                },
        "array": {
            "contains": "İçerir",
        "empty": "Boş",
        "equals": "Eşittir",
        "not": "Değildir",
        "notEmpty": "Dolu",
        "without": "Hariç"
                                }
                            },
        "data": "Veri",
        "deleteTitle": "Filtreleme kuralını silin",
        "leftTitle": "Kriteri dışarı çıkart",
        "logicAnd": "ve",
        "logicOr": "veya",
        "rightTitle": "Kriteri içeri al",
        "title": {
            "0": "Arama Oluşturucu",
        "_": "Arama Oluşturucu (%d)"
                            },
        "value": "Değer",
        "clearAll": "Filtreleri Temizle"
                        },
        "searchPanes": {
            "clearMessage": "Hepsini Temizle",
        "collapse": {
            "0": "Arama Bölmesi",
        "_": "Arama Bölmesi (%d)"
                            },
        "count": "{total}",
        "countFiltered": "{shown}\/{total}",
        "emptyPanes": "Arama Bölmesi yok",
        "loadMessage": "Arama Bölmeleri yükleniyor ...",
        "title": "Etkin filtreler - %d"
                        },
        "searchPlaceholder": "Ara",
        "thousands": ".",
        "datetime": {
            "amPm": [
        "öö",
        "ös"
        ],
        "hours": "Saat",
        "minutes": "Dakika",
        "next": "Sonraki",
        "previous": "Önceki",
        "seconds": "Saniye",
        "unknown": "Bilinmeyen"
                        },
        "decimal": ",",
        "editor": {
            "close": "Kapat",
        "create": {
            "button": "Yeni",
        "submit": "Kaydet",
        "title": "Yeni kayıt oluştur"
                            },
        "edit": {
            "button": "Düzenle",
        "submit": "Güncelle",
        "title": "Kaydı düzenle"
                            },
        "error": {
            "system": "Bir sistem hatası oluştu (Ayrıntılı bilgi)"
                            },
        "multi": {
            "info": "Seçili kayıtlar bu alanda farklı değerler içeriyor. Seçili kayıtların hepsinde bu alana aynı değeri atamak için buraya tıklayın; aksi halde her kayıt bu alanda kendi değerini koruyacak.",
        "noMulti": "Bu alan bir grup olarak değil ancak tekil olarak düzenlenebilir.",
        "restore": "Değişiklikleri geri al",
        "title": "Çoklu değer"
                            },
        "remove": {
            "button": "Sil",
        "confirm": {
            "_": "%d adet kaydı silmek istediğinize emin misiniz?",
        "1": "Bu kaydı silmek istediğinizden emin misiniz?"
                                },
        "submit": "Sil",
        "title": "Kayıtları sil"
                            }
                        }
                    }

                });
        /* DataTables ends here*/

        /* Ajax POST / Deleting a User starts here */
        $(document).on('click', '.btn-delete', function (event) {
            event.preventDefault();
        const id = $(this).attr("data-id"); // silme butonuna bas. user id değer. almak icin..
        const tableRow = $(`[name="${id}"]`); //silinen user satırını seciyoruz..
        const userName = tableRow.find('td:eq(1)').text(); // ikinci sıradaki td user. name ini verd. için
        console.log(tableRow);
        Swal.fire({ //projemize ekled. sweetalert kütüph. kodları
            title: 'Silmek istediğinize emin misiniz?',
        text: `${userName} adlı kategori silinicektir!`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Evet,silmek istiyorum!!',
        cancelButtonText:'Hayır,silmek istemiyorum.'
                    }).then((result) => {
                        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                dataType: 'json',
                data: { userId: id },
                url: '@Url.Action("Delete","User")', // /Admin/User/Delete/
                success: function (data) {
                    const userDto = jQuery.parseJSON(data);
                    if (userDto.ResultStatus === 0) { //silme isl. basa. old. zaman
                        Swal.fire( //sweetalert kütüp. basarılı islemde cıkacak olan alert
                            'Silindi!',
                            `${userDto.User.UserName} adlı kullanıcı başarıyla silinmiştir.`,
                            'success'
                        );

                        dataTable.row(tableRow).remove().draw(); //efektli olarak ekrandan git.
                    }
                    else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Bir hata olustu.',
                            text: `${userDto.Message}`,
                            footer: '<a href="">Why do I have this issue?</a>'
                        });
                    }
                },
                error: function (err) {
                    console.log(err);
                    Swal.fire({
                        icon: 'error',
                        title: 'Bir hata olustu.',
                        text: err,
                        footer: '<a href="">Why do I have this issue?</a>'
                    });
                }


            });
                        }
                    })
                });


    });
