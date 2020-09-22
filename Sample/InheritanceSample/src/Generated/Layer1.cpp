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
	volatile void** virtualFunctionTable0	 = DependentCode::baselayer::SuperA::GetVirtualFunctionTable(this);
	std::memcpy(_Private->_VirtualFunctionTables[0], virtualFunctionTable0, size0);
	_Private->_VirtualFunctionTables[0][0] = 0;
	int size1 = sizeof(volatile void*) * numOfBaseMethods[1];
	volatile void** virtualFunctionTable1	 = DependentCode::baselayer::SubA::GetVirtualFunctionTable(this);
	std::memcpy(_Private->_VirtualFunctionTables[1], virtualFunctionTable1, size1);
	_Private->_VirtualFunctionTables[1][0] = 0;
}

Layer1::~Layer1()
{
}

void* Layer1::InitializeLayerdObject(void* obj, int classID)
{
	int layerID = _Private->_ID;
	if (classID == 0)
	{
		::Layer1::SuperA* layerdObject = reinterpret_cast<::Layer1::SuperA*>(obj);
		layerdObject->_Private->_PartialClassMembers[layerID] = new ::Layer1::SuperA::PartialClassMembers();
		layerdObject->_Private->_PartialClassMembers[layerID]->_Layer = this;
		volatile void* vfp = DependentCode::GetLayerdObjectFinalizer(layerdObject);
		layerdObject->_Private->_PartialClassMembers[layerID]->_Finalizer = vfp;
		layerdObject->_RTCOP_InitializePartialClass();
	}
	else if (classID == 1)
	{
		::Layer1::SubA* layerdObject = reinterpret_cast<::Layer1::SubA*>(obj);
		layerdObject->_Private->_PartialClassMembers[layerID] = new ::Layer1::SubA::PartialClassMembers();
		layerdObject->_Private->_PartialClassMembers[layerID]->_Layer = this;
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
void SuperA::m1 ()
{
	SuperA::PartialClassMembers* layer_members = (SuperA::PartialClassMembers*)_Private->_PartialClassMembers[1];
	volatile void* _RTCOP_proceedaddr = RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer(1)->GetVirtualFunctionTableForProceeding(0)[1];
	auto proceed = [this, _RTCOP_proceedaddr]() { RTCOP::Generated::DependentCode::baselayer::SuperA::ExecuteProceed_m1(this, _RTCOP_proceedaddr); };
	printf ( "Layer1::SuperA::m1()\n" ) ;
	proceed ( ) ;

}

void SubA::m1 ()
{
	SubA::PartialClassMembers* layer_members = (SubA::PartialClassMembers*)_Private->_PartialClassMembers[1];
	volatile void* _RTCOP_proceedaddr = RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer(1)->GetVirtualFunctionTableForProceeding(1)[1];
	auto proceed = [this, _RTCOP_proceedaddr]() { RTCOP::Generated::DependentCode::baselayer::SubA::ExecuteProceed_m1(this, _RTCOP_proceedaddr); };
	volatile void** _RTCOP_vft = RTCOP::Framework::Instance->GetRTCOPManager()->GetVirtualFunctionTable(0);
	auto supercall = [this, _RTCOP_vft]() { RTCOP::Generated::DependentCode::baselayer::SubA::ExecuteProceed_m1(this, _RTCOP_vft[1]); };
	printf ( "Layer1::SubA::m1()\n" ) ;
	proceed ( ) ;

}


}


namespace Layer1{

SuperA::SuperA()
	: RTCOP::Core::LayerdObject<baselayer::SuperA>()
{
}

void SuperA::_RTCOP_InitializePartialClass()
{
}

void SuperA::_RTCOP_FinalizePartialClass()
{
}



SubA::SubA()
	: RTCOP::Core::LayerdObject<baselayer::SubA>()
{
}

void SubA::_RTCOP_InitializePartialClass()
{
}

void SubA::_RTCOP_FinalizePartialClass()
{
}


}

