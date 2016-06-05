function OrderViewModel(orderReadModel) {
    var readModel = orderReadModel;

    Object.defineProperty(this, 'dataType', {
        get: function () {
            return 'order';
        }
    });

    Object.defineProperty(this, 'id', {
        get: function () {
            return readModel.id;
        }
    });

    Object.defineProperty(this, 'itemsCount', {
        get: function () {
            return readModel.itemsCount;
        }
    });
};