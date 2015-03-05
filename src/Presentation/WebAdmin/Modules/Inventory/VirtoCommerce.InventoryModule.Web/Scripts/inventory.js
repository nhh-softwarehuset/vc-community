﻿//Call this to register our module to main application
var moduleName = "virtoCommerce.inventoryModule";

if (AppDependencies != undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [
    'virtoCommerce.inventoryModule.widgets'
])
//.config(
//  ['$stateProvider', function ($stateProvider) {
//      $stateProvider
//          .state('workspace.inventoryModule', {
//              url: '/inventory',
//              templateUrl: 'Modules/Inventory/VirtoCommerce.InventoryModule.Web/Scripts/home.tpl.html',
//              controller: [
//                  '$scope', 'bladeNavigationService', function ($scope, bladeNavigationService) {
//                      var blade = {
//                          id: 'inventory',
//                          title: 'inventory',
//                          controller: 'inventoryListController',
//                          template: 'Modules/Inventory/VirtoCommerce.InventoryModule.Web/Scripts/blades/! -list.tpl.html',
//                          isClosingDisabled: true
//                      };
//                      bladeNavigationService.showBlade(blade);
//                  }
//              ]
//          });
//  }]
//)
.run(
  ['$rootScope', 'mainMenuService', 'widgetService', '$state', function ($rootScope, mainMenuService, widgetService, $state) {
      //Register module in main menu
      //var menuItem = {
      //    path: 'browse/inventory',
      //    icon: 'fa fa-shopping-cart',
      //    title: 'inventorys',
      //    priority: 110,
      //    action: function () { $state.go('workspace.inventoryModule'); },
      //    permission: 'inventoryMenuPermission'
      //};
      //mainMenuService.addMenuItem(menuItem);

      //Register widgets in catalog item details
      widgetService.registerWidget({
          controller: 'inventoryWidgetController',
          template: 'Modules/Inventory/VirtoCommerce.InventoryModule.Web/Scripts/widgets/inventoryWidget.tpl.html'
      }, 'itemDetail');
  }])
;