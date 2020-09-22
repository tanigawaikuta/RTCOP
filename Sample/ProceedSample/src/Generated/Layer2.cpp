#include "./Generated/API.h"
#include "./Generated/Layer_Private.h"
#include "./Generated/LayerdObject_Private.h"
#include "RTCOP/Framework.h"
#include "RTCOP/Core/RTCOPManager.h"
#include "./Generated/DependentCode.h"
#include <iostream>
#include <cstring>

#include "stdio.h"

#include "./Generated/Layer2.h"

namespace RTCOP {
namespace Generated {

Layer2* Layer2::GetInstance()
{
	return (Layer2*)RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer((int)LayerID::Layer2);
}

Layer2::Layer2(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods)
	:RTCOP::Core::Layer(id, name, numOfBaseClasses, numOfBaseMethods)
{
	int size0 = sizeof(volatile void*) * numOfBaseMethods[0];
	volatile void** virtualFunctionTable0	 = DependentCode::baselayer::A::GetVirtualFunctionTable(this);
	std::memcpy(_Private->_VirtualFunctionTables[0], virtualFunctionTable0, size0);
	_Private->_VirtualFunctionTables[0][0] = 0;
	int size1 = sizeof(volatile void*) * numOfBaseMethods[1];
	volatile void** virtualFunctionTable1	 = DependentCode::baselayer::B::GetVirtualFunctionTable(this);
	std::memcpy(_Private->_VirtualFunctionTables[1], virtualFunctionTable1, size1);
	_Private->_VirtualFunctionTables[1][0] = 0;
}

Layer2::~Layer2()
{
}

void* Layer2::InitializeLayerdObject(void* obj, int classID)
{
	int layerID = _Private->_ID;
	if (classID == 0)
	{
		::Layer2::A* layerdObject = reinterpret_cast<::Layer2::A*>(obj);
		layerdObject->_Private->_PartialClassMembers[layerID] = new ::Layer2::A::PartialClassMembers();
		layerdObject->_Private->_PartialClassMembers[layerID]->_Layer = this;
		volatile void* vfp = DependentCode::GetLayerdObjectFinalizer(layerdObject);
		layerdObject->_Private->_PartialClassMembers[layerID]->_Finalizer = vfp;
		layerdObject->_RTCOP_InitializePartialClass();
	}
	else if (classID == 1)
	{
		::Layer2::B* layerdObject = reinterpret_cast<::Layer2::B*>(obj);
		layerdObject->_Private->_PartialClassMembers[layerID] = new ::Layer2::B::PartialClassMembers();
		layerdObject->_Private->_PartialClassMembers[layerID]->_Layer = this;
		volatile void* vfp = DependentCode::GetLayerdObjectFinalizer(layerdObject);
		layerdObject->_Private->_PartialClassMembers[layerID]->_Finalizer = vfp;
		layerdObject->_RTCOP_InitializePartialClass();
	}
	return obj;
}

void Layer2::_RTCOP_OnActivating()
{
}

void Layer2::_RTCOP_OnActivated()
{
}

void Layer2::_RTCOP_OnDeactivating()
{
}

void Layer2::_RTCOP_OnDeactivated()
{
}

} // namespace Generated {}
} // namespace RTCOP {}

namespace Layer2
{
void A::m1 ()
{
	A::PartialClassMembers* layer_members = (A::PartialClassMembers*)_Private->_PartialClassMembers[2];
	volatile void* _RTCOP_proceedaddr = RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer(2)->GetVirtualFunctionTableForProceeding(0)[1];
	auto proceed = [this, _RTCOP_proceedaddr]() { RTCOP::Generated::DependentCode::baselayer::A::ExecuteProceed_m1(this, _RTCOP_proceedaddr); };
	printf ( "Layer2::A::m1()\n" ) ;
	proceed ( ) ;

}

void B::m2 ()
{
	B::PartialClassMembers* layer_members = (B::PartialClassMembers*)_Private->_PartialClassMembers[2];
	volatile void* _RTCOP_proceedaddr = RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer(2)->GetVirtualFunctionTableForProceeding(1)[1];
	auto proceed = [this, _RTCOP_proceedaddr]() { RTCOP::Generated::DependentCode::baselayer::B::ExecuteProceed_m2(this, _RTCOP_proceedaddr); };
	printf ( "Layer2::B::m2()\n" ) ;
	proceed ( ) ;

}


}


namespace Layer2{

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



B::B()
	: RTCOP::Core::LayerdObject<baselayer::B>()
{
}

void B::_RTCOP_InitializePartialClass()
{
}

void B::_RTCOP_FinalizePartialClass()
{
}


}

