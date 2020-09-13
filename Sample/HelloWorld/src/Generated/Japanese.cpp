#include "./src/Generated/API.h"
#include "./src/Generated/Layer_Private.h"
#include "./src/Generated/LayerdObject_Private.h"
#include "RTCOP/Framework.h"
#include "RTCOP/Core/RTCOPManager.h"
#include "./src/Generated/DependentCode.h"
#include <iostream>
#include <cstring>

#include "stdio.h"

#include "./src/Generated/BaseLayer.h"
#include "./src/Generated/English.h"
#include "./src/Generated/Japanese.h"

namespace RTCOP {
namespace Generated {

Japanese* Japanese::GetInstance()
{
	return (Japanese*)RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer((int)LayerID::Japanese);
}

Japanese::Japanese(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods)
	:RTCOP::Core::Layer(id, name, numOfBaseClasses, numOfBaseMethods)
{
	int size0 = sizeof(volatile void*) * numOfBaseMethods[0];
	volatile void** virtualFunctionTable0	 = DependentCode::baselayer::Hello::GetVirtualFunctionTable(this);
	std::memcpy(_Private->_VirtualFunctionTables[0], virtualFunctionTable0, size0);
	_Private->_VirtualFunctionTables[0][1] = 0;
}

Japanese::~Japanese()
{
}

void* Japanese::InitializeLayerdObject(void* obj, int classID)
{
	int layerID = _Private->_ID;
	if (classID == 0)
	{
		::Japanese::Hello* layerdObject = reinterpret_cast<::Japanese::Hello*>(obj);
		layerdObject->_Private->_PartialClassMembers[layerID] = new ::Japanese::Hello::PartialClassMembers();
		layerdObject->_Private->_PartialClassMembers[layerID]->_Layer = this;
		layerdObject->_Private->_PartialClassMembers[layerID]->_VirtualFunctionTableForProceeding = _Private->_VirtualFunctionTablesForProceeding[classID];
		volatile void* vfp = DependentCode::GetLayerdObjectFinalizer(layerdObject);
		layerdObject->_Private->_PartialClassMembers[layerID]->_Finalizer = vfp;
		layerdObject->_RTCOP_InitializePartialClass();
	}
	return obj;
}

void Japanese::_RTCOP_OnActivating()
{
}

void Japanese::_RTCOP_OnActivated()
{
}

void Japanese::_RTCOP_OnDeactivating()
{
}

void Japanese::_RTCOP_OnDeactivated()
{
}

} // namespace Generated {}
} // namespace RTCOP {}

namespace Japanese
{
void Hello::Print ()
{
	Hello::PartialClassMembers* layer_members = (Hello::PartialClassMembers*)_Private->_PartialClassMembers[2];
	auto proceed = [this, layer_members]() { RTCOP::Generated::DependentCode::baselayer::Hello::ExecuteProceed_Print(this, layer_members->_VirtualFunctionTableForProceeding[0]); };
	printf ( "こんにちは世界\n" ) ;
	proceed ( ) ;

}

}


namespace Japanese{

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

