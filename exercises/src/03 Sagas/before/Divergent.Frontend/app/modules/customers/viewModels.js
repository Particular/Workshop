function CustomerViewModel(customerReadModel) {
    var readModel = customerReadModel;

    Object.defineProperty(this, 'dataType', {
        get: function () {
            return 'customer';
        }
    });

    Object.defineProperty(this, 'displayName', {
        get: function () {
            return readModel.name;
        }
    });

    Object.defineProperty(this, 'id', {
        get: function () {
            return readModel.id;
        }
    });
};