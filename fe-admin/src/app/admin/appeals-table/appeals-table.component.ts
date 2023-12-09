import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTable } from '@angular/material/table';
import {
  AppealsTableDataSource,
  iAppeals,
} from './appeals-table-datasource';

@Component({
  selector: 'app-appeals-table',
  templateUrl: './appeals-table.component.html',
  styleUrls: ['./appeals-table.component.scss'],
})
export class AppealsTableComponent implements AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatTable) table!: MatTable<iAppeals>;
  dataSource: AppealsTableDataSource;

  /** Columns displayed in the table. Columns IDs can be added, removed, or reordered. */
  displayedColumns = ['id', 'Status', 'RegistrationDate', 'CloseDate', 'AppealsType', 'chat', 'options'];

  constructor() {
    this.dataSource = new AppealsTableDataSource();
  }

  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
    this.table.dataSource = this.dataSource;
  }

  edit(id:number) {}

  delete(id:number) {}
}
