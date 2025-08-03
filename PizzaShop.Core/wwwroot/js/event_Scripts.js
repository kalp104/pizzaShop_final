



$(document).ready(function(e){

    $("#Show_EventListPartial").show();
    $("#show_AddEventPartial").hide();

    $(document).on('click','#AddEventBtn',function(e){
        e.preventDefault();
        $("#Show_EventListPartial").hide();
        $("#show_AddEventPartial").show();
    });
    $(document).on('click','#BackToEventBtn',function(){
        $("#Show_EventListPartial").show();
        $("#show_AddEventPartial").hide();
    });
});