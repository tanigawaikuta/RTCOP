#include "./Generated/API.h"
#include "./Generated/Layer_Private.h"
#include "./Generated/LayerdObject_Private.h"
#include "RTCOP/Framework.h"
#include "RTCOP/Core/RTCOPManager.h"
#include "./Generated/DependentCode.h"
#include <iostream>
#include <cstring>

#include "stdio.h"

#include "./Generated/Layer1.h"

namespace RTCOP {
namespace Generated {

Layer1* Layer1::GetInstance()
{
	return (Layer1*)RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer((int)LayerID::Layer1);
}

Layer1::Layer1(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods)
	:RTCOP::Core::Layer(id, name, numOfBaseClasses, numOfBaseMethods)
{
	int size0 = sizeof(volatile void*) * numOfBaseMethods[0];
	volatile void** virtualFunctionTable0	 = DependentCode::baselayer::A::GetVirtualFunctionTable(this);
	std::memcpy(_Private->_VirtualFunctionTables[0], virtualFunctionTable0, size0);
	_Private->_VirtualFunctionTables[0][0] = 0;
}

Layer1::~Layer1()
{
}

void* Layer1::InitializeLayerdObject(void* obj, int classID)
{
	int layerID = _Private->_ID;
	if (classID == 0)
	{
		::Layer1::A* layerdObject = reinterpret_cast<::Layer1::A*>(obj);
		layerdObject->_Private->_PartialClassMembers[layerID] = new ::Layer1::A::PartialClassMembers();
		layerdObject->_Private->_PartialClassMembers[layerID]->_Layer = this;
		layerdObject->_Private->_PartialClassMembers[layerID]->_VirtualFunctionTableForProceeding = _Private->_VirtualFunctionTablesForProceeding[classID];
		volatile void* vfp = DependentCode::GetLayerdObjectFinalizer(layerdObject);
		layerdObject->_Private->_PartialClassMembers[layerID]->_Finalizer = vfp;
		layerdObject->_RTCOP_InitializePartialClass();
	}
	return obj;
}

void Layer1::_RTCOP_OnActivating()
{
}

void Layer1::_RTCOP_OnActivated()
{
}

void Layer1::_RTCOP_OnDeactivating()
{
}

void Layer1::_RTCOP_OnDeactivated()
{
}

} // namespace Generated {}
} // namespace RTCOP {}

namespace Layer1
{
void A::m1 ()
{
	A::PartialClassMembers* layer_members = (A::PartialClassMembers*)_Private->_PartialClassMembers[1];
	auto proceed = [this, layer_members]() { RTCOP::Generated::DependentCode::baselayer::A::ExecuteProceed_m1(this, layer_members->_VirtualFunctionTableForProceeding[1]); };
	printf ( "Layer1::A::m1()\n" ) ;
	proceed ( ) ;

}

}


namespace Layer1{

A::A()
	: RTCOP::Core::LayerdObject<baselayer::A>()
{
}

void A::_RTCOP_InitializePartialClass()
{
}

void A::_RTCOP_FinalizePartialClass()
{
}


}

