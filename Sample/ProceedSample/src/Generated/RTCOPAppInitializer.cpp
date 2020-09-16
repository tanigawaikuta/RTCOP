#include "./Generated/RTCOPAppInitializer.h"
#include "RTCOP/Core/RTCOPManager.h"
#include "RTCOP/Core/LayerdObjectInitializer.h"
#include "RTCOP/Core/LayerActivater.h"
#include "./Generated/BaseLayer.h"
#include "./Generated/Layer1.h"
#include "./Generated/Layer2.h"

namespace RTCOP {
namespace Generated {

RTCOPAppInitializer::RTCOPAppInitializer()
{
}

RTCOPAppInitializer::~RTCOPAppInitializer()
{
}

Core::RTCOPManager* RTCOPAppInitializer::InitializeRTCOPManager()
{
	int numOfLayers = 3;	int numOfClasses = 2;	int* numOfMethods = new int[numOfClasses] {2, 2};
	return new Core::RTCOPManager(numOfLayers, numOfClasses, numOfMethods);
}

Core::LayerdObjectInitializer* RTCOPAppInitializer::InitializeLayerdObjectInitializer(Core::RTCOPManager* manager)
{
	return new Core::LayerdObjectInitializer(manager);
}

Core::LayerActivater* RTCOPAppInitializer::InitializeLayerActivater(Core::RTCOPManager* manager)
{
	return new Core::LayerActivater(manager);
}

void RTCOPAppInitializer::RegisterLayers(Core::RTCOPManager* manager)
{
	int numOfBaseClasses = manager->GetNumOfBaseClasses();
	int* numOfBaseMethods = manager->GetNumOfBaseMethods();
	manager->RegisterLayer(new BaseLayer(0, "baselayer", numOfBaseClasses, numOfBaseMethods));
	manager->RegisterLayer(new Layer1(1, "Layer1", numOfBaseClasses, numOfBaseMethods));
	manager->RegisterLayer(new Layer2(2, "Layer2", numOfBaseClasses, numOfBaseMethods));
}

void RTCOPAppInitializer::FirstLayerActivation(Core::LayerActivater* activater)
{
	activater->ActivateLayer(0);
}

Core::Initializer* _GetDefaultInitializer_()
{
	return new RTCOPAppInitializer();
}

} // namespace Generated {}
} // namespace RTCOP {}
