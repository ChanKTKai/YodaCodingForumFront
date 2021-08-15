$(document).ready(function() {  
    function likeClick(type, targetType, userDo, clickItem) {
        $.post("/article/userCollection", {
            type: type,
            userName: "@userName",
            targetID: $(clickItem).prev("div").text(),
            targetType: targetType,
            userDo: userDo
        }, function (response) {
            switch (response) {
                case "Success":
                    switch (type) {
                        case "like":
                            $(clickItem).children().first().toggleClass("fa-thumbs-up");
                            $(clickItem).children().first().toggleClass("fa-thumbs-o-up");
                            break;
                        case "collect":
                            $(clickItem).children().first().toggleClass("fa-star");
                            $(clickItem).children().first().toggleClass("fa-star-o");
                            $(clickItem).children().first().toggleClass("starLight");
                            break;
                        case "follow":
                            $(clickItem).children().first().toggleClass("fa-heart");
                            $(clickItem).children().first().toggleClass("fa-heart-o");
                            $(clickItem).children().first().toggleClass("heartLight");
                            break;
                        default:
                            break;
                    };
                    break;
                default:
                    alert("錯誤");
                    break;
            }
        });
    };
    $(function () {

        //將要執行頁碼的TableID放入呼叫func
        getPagination('#userList');

        //判斷Table列數產生頁碼
        function getPagination(table) {

            //將class:pagination內的html初始
            $('.pagination').html('');
            //一頁最多顯示的列數
            var maxRows = 10;
            //所有資料列數
            var totalRows = $(table + ' .card').length;


            /*console.log(totalRows);*/
            //資料列數變數初始
            var trnum = 0;
            //Table跑每一列資料<tr>
            $(table + ' .card').each(function () {

                trnum++;

                //如果超過設定的顯示列數
                if (trnum > maxRows) {

                    $(this).hide();

                } else {

                    if (trnum <= maxRows) { $(this).show(); }
                }

            });


            //如總列數超過設定顯示列數
            if (totalRows > maxRows) {

                //  總頁數 = ( 總列數 除 顯示列數 ) 無條件進位 。
                var pagenum = Math.ceil(totalRows / maxRows);

                //從1開始至總頁數
                for (var i = 1; i <= pagenum;) {

                    //在class:pagination內加入<li>頁碼
                    $('.pagination').append('<li class="page-item" data-page="' + i + '">\<a class="page-link " href="#">' + i++ + '\</a>\</li>'
                    ).show();
                }
                $('.pagination li:first-child').addClass('active');

            }


            //頁碼的點擊事件
            $('.page-item').on('click', function (e) {

                //阻止表單的提交
                e.preventDefault();
                //取得頁碼
                var pageNum = $(this).attr('data-page');


                $('.pagination li').removeClass('active');
                $(this).addClass('active');

                //資料列數變數初始
                var trIndex = 0;
                //Table跑每一列資料<tr>
                $(table + ' .card').each(function () {

                    trIndex++;

                    //判斷 列數是否超過 最大顯示列數*頁碼   或   小於等於 最大顯示列數*頁碼-最大顯示列數
                    //ex. (最大顯示列數為10) 點擊第二頁時 若資料列數 > 20 或 <= 10 的資料都被隱藏 ==> 顯示11-20的資料
                    if (trIndex > (maxRows * pageNum) || trIndex <= ((maxRows * pageNum) - maxRows)) {
                        $(this).hide();
                    } else {
                        $(this).show();
                    }
                });
            });
            //頁碼的點擊事件-END


        }
    });
});  
  

