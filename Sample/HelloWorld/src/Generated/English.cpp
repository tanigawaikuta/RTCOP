#include "./Generated/API.h"
#include "./Generated/Layer_Private.h"
#include "./Generated/LayerdObject_Private.h"
#include "RTCOP/Framework.h"
#include "RTCOP/Core/RTCOPManager.h"
#include "./Generated/DependentCode.h"
#include <iostream>
#include <cstring>

#include "stdio.h"

#include "./Generated/BaseLayer.h"
#include "./Generated/Japanese.h"
#include "./Generated/English.h"

namespace RTCOP {
namespace Generated {

English* English::GetInstance()
{
	return (English*)RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer((int)LayerID::English);
}

English::English(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods)
	:RTCOP::Core::Layer(id, name, numOfBaseClasses, numOfBaseMethods)
{
	int size0 = sizeof(volatile void*) * numOfBaseMethods[0];
	volatile void** virtualFunctionTable0	 = DependentCode::baselayer::Hello::GetVirtualFunctionTable(this);
	std::memcpy(_Private->_VirtualFunctionTables[0], virtualFunctionTable0, size0);
	_Private->_VirtualFunctionTables[0][1] = 0;
}

English::~English()
{
}

void* English::InitializeLayerdObject(void* obj, int classID)
{
	int layerID = _Private->_ID;
	if (classID == 0)
	{
		::English::Hello* layerdObject = reinterpret_cast<::English::Hello*>(obj);
		layerdObject->_Private->_PartialClassMembers[layerID] = new ::English::Hello::PartialClassMembers();
		layerdObject->_Private->_PartialClassMembers[layerID]->_Layer = this;
		volatile void* vfp = DependentCode::GetLayerdObjectFinalizer(layerdObject);
		layerdObject->_Private->_PartialClassMembers[layerID]->_Finalizer = vfp;
		layerdObject->_RTCOP_InitializePartialClass();
	}
	return obj;
}

void English::_RTCOP_OnActivating()
{
}

void English::_RTCOP_OnActivated()
{
}

void English::_RTCOP_OnDeactivating()
{
}

void English::_RTCOP_OnDeactivated()
{
}

} // namespace Generated {}
} // namespace RTCOP {}

namespace English
{
void Hello::Print ()
{
	Hello::PartialClassMembers* layer_members = (Hello::PartialClassMembers*)_Private->_PartialClassMembers[1];
	volatile void* _RTCOP_proceedaddr = RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer(1)->GetVirtualFunctionTableForProceeding(0)[0];
	auto proceed = [this, _RTCOP_proceedaddr]() { RTCOP::Generated::DependentCode::baselayer::Hello::ExecuteProceed_Print(this, _RTCOP_proceedaddr); };
	printf ( "Hello World\n" ) ;

}


}


namespace English{

Hello::Hello()
	: RTCOP::Core::LayerdObject<baselayer::Hello>()
{
}

void Hello::_RTCOP_InitializePartialClass()
{
}

void Hello::_RTCOP_FinalizePartialClass()
{
}


}

