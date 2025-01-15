function confirmDelete(uniqueId, isDeleted) {
    var deleteSpan = "DeleteSpan_" + uniqueId;
    var confirmDeleteSpan = "ConfirmDeleteSpan_" + uniqueId;

    if (isDeleted) {
        $('#' + deleteSpan).hide();
        $('#' + confirmDeleteSpan).show();
    } else {
        $('#' + deleteSpan).show();
        $('#' + confirmDeleteSpan).hide();
    }
}
