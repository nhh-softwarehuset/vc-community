﻿//Call this to register our module to main application
var moduleName = "virtoCommerce.content.menuModule";

if (AppDependencies != undefined) {
	AppDependencies.push(moduleName);
}

angular.module(moduleName, [
	'virtoCommerce.content.menuModule.widgets.menuWidget',
	'virtoCommerce.content.menuModule.resources.menus',
	'virtoCommerce.content.menuModule.blades.linkLists',
	'virtoCommerce.content.menuModule.blades.menuLinkList'
])
.run(['$rootScope', 'mainMenuService', 'widgetService', '$state', function ($rootScope, mainMenuService, widgetService, $state) {

	//Register widgets in store details
	widgetService.registerWidget({
		controller: 'menuWidgetController',
		template: 'Modules/CMS/VirtoCommerce.MenuModule.Web/Scripts/widgets/menuWidget.tpl.html'
	}, 'storeDetail');

}])
;