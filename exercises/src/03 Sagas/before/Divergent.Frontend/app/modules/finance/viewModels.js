function PriceViewModel(priceReadModel) {
    var readModel = priceReadModel;

    Object.defineProperty(this, 'dataType', {
        get: function () {
            return 'price';
        }
    });

    Object.defineProperty(this, 'value', {
        get: function () {
            return priceReadModel;
        }
    });

    Object.defineProperty(this, 'currency', {
        get: function () {
            return '$';
        }
    });
};