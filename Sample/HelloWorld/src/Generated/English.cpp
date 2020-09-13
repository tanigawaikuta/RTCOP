#include "./src/Generated/API.h"
#include "./src/Generated/Layer_Private.h"
#include "./src/Generated/LayerdObject_Private.h"
#include "RTCOP/Framework.h"
#include "RTCOP/Core/RTCOPManager.h"
#include "./src/Generated/DependentCode.h"
#include <iostream>
#include <cstring>


#include "./src/Generated/English.h"

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
		layerdObject->_Private->_PartialClassMembers[layerID]->_VirtualFunctionTableForProceeding = _Private->_VirtualFunctionTablesForProceeding[classID];
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

#include "stdio.h"
namespace English
{
void Hello::Print ()
{
	Hello::PartialClassMembers* layer_members = (Hello::PartialClassMembers*)_Private->_PartialClassMembers[1];
	auto proceed = [this, layer_members]() { RTCOP::Generated::DependentCode::baselayer::Hello::ExecuteProceed_Print(this, layer_members->_VirtualFunctionTableForProceeding[0]); };
	printf ( "Hello World\n" ) ;
	proceed ( ) ;

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

