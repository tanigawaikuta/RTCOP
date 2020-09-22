#include "./Generated/API.h"
#include "./Generated/Layer_Private.h"
#include "./Generated/LayerdObject_Private.h"
#include "./Generated/DependentCode.h"
#include "RTCOP/Framework.h"
#include "RTCOP/Core/RTCOPManager.h"
#include "RTCOP/Core/PartialClassMembers.h"
#include <cstring>


#include "./Generated/BaseLayer.h"

namespace RTCOP {
namespace Generated {

BaseLayer* BaseLayer::GetInstance()
{
	return (BaseLayer*)RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer((int)LayerID::baselayer);
}

BaseLayer::BaseLayer(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods)
	:Core::Layer(id, name, numOfBaseClasses, numOfBaseMethods)
{
	int size0 = sizeof(volatile void*) * numOfBaseMethods[0];
	volatile void** virtualFunctionTable0 = DependentCode::Hello::GetVirtualFunctionTable(this);
	std::memcpy(_Private->_VirtualFunctionTables[0], virtualFunctionTable0, size0);
}

BaseLayer::~BaseLayer()
{
}

void* BaseLayer::InitializeLayerdObject(void* obj, int classID)
{
	return obj;
}

} // namespace Generated {}
} // namespace RTCOP {}



